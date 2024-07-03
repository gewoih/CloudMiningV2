using CloudMining.Domain.Enums;

namespace CloudMining.Infrastructure.Settings;

public sealed class MarketDataLoaderSettings
{
    public static readonly string SectionName = "MarketDataLoader";
    public TimeSpan Delay { get; set; }
    public TimeSpan CbrDelay { get; set; }
    public List<CurrencyPair> CurrencyPairs { get; set; }
}

public sealed class CurrencyPair
{
    public CurrencyCode From { get; set; }
    public CurrencyCode To { get; set; }
}