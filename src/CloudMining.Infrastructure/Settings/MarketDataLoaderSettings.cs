namespace CloudMining.Infrastructure.Settings;

public class MarketDataLoaderSettings
{
    public static readonly string SectionName = "MarketDataLoader";
    public int DelayInMinutes { get; set; }
}