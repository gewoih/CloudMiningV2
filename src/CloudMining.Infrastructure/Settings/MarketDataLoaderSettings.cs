using CloudMining.Interfaces.DTO.Currencies;

namespace CloudMining.Infrastructure.Settings;

public sealed class MarketDataLoaderSettings
{
    public static readonly string SectionName = "MarketDataLoader";
    public TimeSpan Delay { get; set; }
    public List<CurrencyPair> CurrencyPairs { get; set; }
}