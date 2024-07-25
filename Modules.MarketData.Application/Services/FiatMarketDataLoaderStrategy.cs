using CloudMining.Common.Settings;
using Modules.MarketData.Contracts.Interfaces;
using Modules.MarketData.Infrastructure.CentralBankRussia;
using Modules.MarketData.Infrastructure.Settings;

namespace Modules.MarketData.Application.Services;

public sealed class FiatMarketDataLoaderStrategy : IMarketDataLoaderStrategy
{
	private readonly CentralBankRussiaApiClient _centralBankRussiaApiClient;

	public FiatMarketDataLoaderStrategy(CentralBankRussiaApiClient centralBankRussiaApiClient)
	{
		_centralBankRussiaApiClient = centralBankRussiaApiClient;
	}

	public async Task<List<Domain.Models.MarketData>> GetMarketDataAsync(CurrencyPair currencyPair, 
		DateTime? fromDate = null,
		DateTime? toDate = null)
	{
		var loadedMarketData = new List<Domain.Models.MarketData>();

		var centralBankRussiaMarketData = await _centralBankRussiaApiClient.GetMarketDataAsync(fromDate, toDate);

		if (centralBankRussiaMarketData.Count == 0)
			return loadedMarketData;

		loadedMarketData.AddRange(centralBankRussiaMarketData.Select(data => new Domain.Models.MarketData
		{
			From = currencyPair.From,
			To = currencyPair.To,
			Price = data.Price,
			Date = data.Date
		}));

		return loadedMarketData;
	}
}