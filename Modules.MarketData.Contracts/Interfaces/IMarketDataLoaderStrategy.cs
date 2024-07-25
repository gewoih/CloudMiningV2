using CloudMining.Common.Settings;
using Modules.MarketData.Infrastructure.Settings;

namespace Modules.MarketData.Contracts.Interfaces;

public interface IMarketDataLoaderStrategy
{
	Task<List<CloudMining.Common.Models.Currencies.MarketData>> GetMarketDataAsync(CurrencyPair currencyPair, DateTime? fromDate = null, DateTime? toDate = null);
}