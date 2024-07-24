using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Infrastructure.Settings;

namespace CloudMining.Interfaces.Interfaces;

public interface IMarketDataService
{
    Task SaveMarketDataAsync(List<MarketData> marketData);
    Task<DateTime?> GetLastMarketDataDateAsync(CurrencyCode fromCurrency, CurrencyCode toCurrency);

    Task<Dictionary<CurrencyPair, MarketData?>> GetLatestMarketDataForCurrenciesAsync(
        IEnumerable<CurrencyPair> currencyPairs);
}