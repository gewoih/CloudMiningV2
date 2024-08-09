using System.Text.Json.Nodes;
using CloudMining.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CloudMining.Infrastructure.Emcd;

public sealed class EmcdApiClient
{
	private readonly string _apiKey;
	private readonly List<string> _availableCoins;
	private readonly string _getPayoutsUrl;
	private readonly HttpClient _httpClient;

	public EmcdApiClient(HttpClient httpClient, IOptions<EmcdSettings> settings)
	{
		_httpClient = httpClient;

		var baseUrl = settings.Value.BaseUrl;
		_getPayoutsUrl = baseUrl + settings.Value.Endpoints.GetPayoutsUrl;
		_apiKey = settings.Value.ApiKey;
		_availableCoins = settings.Value.AvailableCoins;
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

	private static List<Payout> GetFilteredPayoutsByDate(List<Payout> payouts, DateTime? fromDate = null,
		DateTime? toDate = null)
	{
		if (fromDate.HasValue)
			payouts = payouts.Where(p => p.GmtTime > fromDate.Value).ToList();
		if (toDate.HasValue)
			payouts = payouts.Where(p => p.GmtTime <= toDate.Value).ToList();

		payouts = payouts.OrderBy(payout => payout.GmtTime).ToList();

		return payouts;
	}
}