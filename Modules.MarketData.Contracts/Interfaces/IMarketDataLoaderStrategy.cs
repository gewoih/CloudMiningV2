using CloudMining.Common.Settings;
using Modules.MarketData.Infrastructure.Settings;

namespace Modules.MarketData.Contracts.Interfaces;

public interface IMarketDataLoaderStrategy
{
	Task<List<Domain.Models.MarketData>> GetMarketDataAsync(CurrencyPair currencyPair, DateTime? fromDate = null, DateTime? toDate = null);
}