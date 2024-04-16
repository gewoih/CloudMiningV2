using System.Text.Json.Nodes;
using CloudMining.Domain.Enums;
using CloudMining.Infrastructure.EMCD.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CloudMining.Infrastructure.EMCD.Services
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

		public async Task<List<Payout>> GetPayouts(CurrencyCode? currencyCode = null, DateTime? fromDate = null, DateTime? toDate = null)
		{
			var getPayoutsTasks = new List<Task<HttpResponseMessage>>();
			foreach (var coinName in _availableCoins)
			{
				var requestUrl = string.Format(_getPayoutsUrl, coinName, _apiKey);
				getPayoutsTasks.Add(_httpClient.GetAsync(requestUrl));
			}
			await Task.WhenAll(getPayoutsTasks);

			var payouts = new List<Payout>();
;			foreach (var httpResponseMessage in getPayoutsTasks.Select(task => task.Result))
			{
				var stringResponse = await httpResponseMessage.Content.ReadAsStringAsync();
				var jsonPayouts = JsonObject.Parse(stringResponse)["payouts"].ToJsonString();
				var newPayouts = JsonConvert.DeserializeObject<List<Payout>>(jsonPayouts);

				if (newPayouts is not null)
					payouts.AddRange(newPayouts);
			}

			if (fromDate != null)
				payouts = payouts.Where(payout => payout.GmtTime >= fromDate).ToList();

			if (toDate != null)
				payouts = payouts.Where(payout => payout.GmtTime <= toDate).ToList();

			return payouts;
		}
	}
}
