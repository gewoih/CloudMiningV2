using CloudMining.Domain.Enums;

namespace CloudMining.Infrastructure.Settings;

public class MarketDataLoaderSettings
{
    public static readonly string SectionName = "MarketDataLoader";
    public int DelayInMinutes { get; set; }
    public List<CurrencyPairs> CurrencyPairs { get; set; }
}

public class CurrencyPairs
{
    public CurrencyCode From { get; set; }
    public CurrencyCode To { get; set; }
}