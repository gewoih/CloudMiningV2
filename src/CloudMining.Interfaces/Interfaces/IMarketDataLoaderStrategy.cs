using CloudMining.Domain.Models.Currencies;
using CloudMining.Infrastructure.Settings;

namespace CloudMining.Interfaces.Interfaces;

public interface IMarketDataLoaderStrategy
{
	Task<List<MarketData>> GetMarketDataAsync(CurrencyPair currencyPair, DateTime fromDate, DateTime toDate);
}