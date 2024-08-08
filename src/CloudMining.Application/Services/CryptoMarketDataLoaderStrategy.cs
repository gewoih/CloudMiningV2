using CloudMining.Domain.Models.Currencies;
using CloudMining.Infrastructure.Binance;
using CloudMining.Infrastructure.Settings;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.Interfaces;
using Microsoft.Extensions.Options;

namespace CloudMining.Application.Services;

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

	public async Task<List<MarketData>> GetMarketDataAsync(CurrencyPair currencyPair, 
		DateTime? fromDate = null, 
		DateTime? toDate = null)
	{
		var loadedMarketData = new List<MarketData>();
		for (; fromDate < toDate; fromDate += _loadingDelay)
		{
			var binanceMarketData = await _binanceApiClient.GetMarketDataAsync(
				fromCurrency: currencyPair.From,
				toCurrency: currencyPair.To,
				fromDate: fromDate,
				limit: 1000);

			if (binanceMarketData.Count == 0)
				break;

			loadedMarketData.AddRange(binanceMarketData.Select(data => new MarketData
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