using Modules.Currencies.Domain.Enums;

namespace Modules.MarketData.Contracts.Interfaces;

public interface IMarketDataService
{
    Task SaveMarketDataAsync(List<CloudMining.Common.Models.Currencies.MarketData> marketData);
    Task<DateTime?> GetLastMarketDataDateAsync(CurrencyCode fromCurrency, CurrencyCode toCurrency);

}