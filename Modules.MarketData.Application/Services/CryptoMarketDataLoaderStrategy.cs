using CloudMining.Common.Settings;
using Microsoft.Extensions.Options;
using Modules.MarketData.Contracts.Interfaces;
using Modules.MarketData.Infrastructure.Binance;
using Modules.MarketData.Infrastructure.Settings;

namespace Modules.MarketData.Application.Services;

public sealed class CryptoMarketDataLoaderStrategy : IMarketDataLoaderStrategy
{
	private readonly BinanceApiClient _binanceApiClient;
	private readonly TimeSpan _loadingDelay;

	public CryptoMarketDataLoaderStrategy(BinanceApiClient binanceApiClient, 
		IOptions<MarketDataLoaderSettings> settings)
	{
		_binanceApiClient = binanceApiClient;
		_loadingDelay = settings.Value.Delay;
	}

	public async Task<List<CloudMining.Common.Models.Currencies.MarketData>> GetMarketDataAsync(CurrencyPair currencyPair, 
		DateTime? fromDate = null, 
		DateTime? toDate = null)
	{
		var loadedMarketData = new List<CloudMining.Common.Models.Currencies.MarketData>();
		for (; fromDate < toDate; fromDate += _loadingDelay)
		{
			var binanceMarketData = await _binanceApiClient.GetMarketDataAsync(
				fromCurrency: currencyPair.From,
				toCurrency: currencyPair.To,
				fromDate: fromDate,
				limit: 1000);

			if (binanceMarketData.Count == 0)
				break;

			loadedMarketData.AddRange(binanceMarketData.Select(data => new CloudMining.Common.Models.Currencies.MarketData
			{
				From = currencyPair.From,
				To = currencyPair.To,
				Price = data.Price,
				Date = data.Date
			}));

			fromDate = binanceMarketData.Max(data => data.Date);
		}

		return loadedMarketData;
	}
}