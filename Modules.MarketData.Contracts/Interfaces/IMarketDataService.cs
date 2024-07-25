using Modules.Currencies.Domain.Enums;

namespace Modules.MarketData.Contracts.Interfaces;

public interface IMarketDataService
{
    Task SaveMarketDataAsync(List<Domain.Models.MarketData> marketData);
    Task<DateTime?> GetLastMarketDataDateAsync(CurrencyCode fromCurrency, CurrencyCode toCurrency);

}