using CloudMining.Domain.Models.Currencies;
using CloudMining.Interfaces.DTO.Currencies;

namespace CloudMining.Interfaces.Interfaces;

public interface IMarketDataLoaderStrategy
{
	Task<List<MarketData>> GetMarketDataAsync(CurrencyPair currencyPair, DateTime? fromDate = null, DateTime? toDate = null);
}