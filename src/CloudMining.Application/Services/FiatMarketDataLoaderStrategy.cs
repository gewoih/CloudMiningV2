using CloudMining.Domain.Models.Currencies;
using CloudMining.Infrastructure.CentralBankRussia;
using CloudMining.Infrastructure.Settings;
using CloudMining.Interfaces.Interfaces;

namespace CloudMining.Application.Services;

public sealed class FiatMarketDataLoaderStrategy : IMarketDataLoaderStrategy
{
	private readonly CentralBankRussiaApiClient _centralBankRussiaApiClient;

	public FiatMarketDataLoaderStrategy(CentralBankRussiaApiClient centralBankRussiaApiClient)
	{
		_centralBankRussiaApiClient = centralBankRussiaApiClient;
	}

	public async Task<List<MarketData>> GetMarketDataAsync(CurrencyPair currencyPair, DateTime fromDate, DateTime toDate)
	{
		var loadedMarketData = new List<MarketData>();

		var centralBankRussiaMarketData = await _centralBankRussiaApiClient.GetHistoricalMarketDataAsync(fromDate, toDate);

		if (centralBankRussiaMarketData.Count == 0)
			return loadedMarketData;

		loadedMarketData.AddRange(centralBankRussiaMarketData.Select(data => new MarketData
		{
			From = currencyPair.From,
			To = currencyPair.To,
			Price = data.Price,
			Date = data.Date
		}));

		return loadedMarketData;
	}
}