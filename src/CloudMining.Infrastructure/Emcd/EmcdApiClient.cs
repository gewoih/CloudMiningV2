using System.Text.Json.Nodes;
using CloudMining.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CloudMining.Infrastructure.Emcd
{
    public sealed class EmcdApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _getPayoutsUrl;
        private readonly List<string> _availableCoins;

        public EmcdApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            var baseUrl = configuration["Emcd:BaseUrl"];
            _getPayoutsUrl = baseUrl + configuration["Emcd:Endpoints:GetPayoutsUrl"];
            _apiKey = configuration["Emcd:ApiKey"];
            _availableCoins = configuration.GetSection("Emcd:AvailableCoins").Get<List<string>>();
        }

        public async Task<List<Payout>> GetPayouts(DateTime? fromDate = null, DateTime? toDate = null)
        {
			var getPayoutsTasks = _availableCoins.Select(coinName =>
				_httpClient.GetAsync(string.Format(_getPayoutsUrl, coinName, _apiKey))
					.ContinueWith(task => new { CoinName = coinName, Response = task.Result })
			).ToList();

			var responses = await Task.WhenAll(getPayoutsTasks);
            var payouts = new List<Payout>();
            foreach (var coinResponsePair in responses)
            {
                if (!coinResponsePair.Response.IsSuccessStatusCode)
                    continue;

                var stringResponse = await coinResponsePair.Response.Content.ReadAsStringAsync();
                var jsonPayouts = JsonNode.Parse(stringResponse)?["payouts"]?.ToJsonString();
                if (string.IsNullOrEmpty(jsonPayouts))
                    continue;

                var newPayouts = JsonConvert.DeserializeObject<List<Payout>>(jsonPayouts);
                if (newPayouts is null) 
	                continue;
                
                newPayouts.ForEach(p => p.CoinName = coinResponsePair.CoinName);
                payouts.AddRange(newPayouts);
            }

            payouts = GetFilteredPayoutsByDate(payouts, fromDate, toDate);

			return payouts;
        }

        private static List<Payout> GetFilteredPayoutsByDate(List<Payout> payouts, DateTime? fromDate = null, DateTime? toDate = null)
        {
	        if (fromDate.HasValue)
		        payouts = payouts.Where(p => p.GmtTime >= fromDate.Value).ToList();
	        if (toDate.HasValue)
		        payouts = payouts.Where(p => p.GmtTime <= toDate.Value).ToList();

	        payouts = payouts.OrderBy(payout => payout.GmtTime).ToList();

	        return payouts;
        }
    }
}
