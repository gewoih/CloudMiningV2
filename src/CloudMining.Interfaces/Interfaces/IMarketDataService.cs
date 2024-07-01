using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;

namespace CloudMining.Interfaces.Interfaces;

public interface IMarketDataService
{
    Task SaveMarketData(List<MarketData> marketData);
    Task<DateTime?> GetLastMarketDataDate(CurrencyCode fromCurrency, CurrencyCode toCurrency);

}