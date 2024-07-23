using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Payments.Shareable;

namespace CloudMining.Interfaces.Interfaces;

public interface IMarketDataService
{
    Task SaveMarketDataAsync(List<MarketData> marketData);
    Task<DateTime?> GetLastMarketDataDateAsync(CurrencyCode fromCurrency, CurrencyCode toCurrency);
    Task<List<MarketData>> GetCurrencyRatesForPayoutsAsync(IEnumerable<ShareablePayment> payoutsList);
}