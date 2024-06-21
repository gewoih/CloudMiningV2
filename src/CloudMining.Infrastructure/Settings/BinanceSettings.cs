namespace CloudMining.Infrastructure.Settings;

public class BinanceSettings
{
    public static readonly string SectionName = "Binance";
    public string BaseUrl { get; set; }
    public BinanceEndpoints Endpoints { get; set; }
}

public class BinanceEndpoints
{
    public string GetPriceDataUrl { get; set; }
}