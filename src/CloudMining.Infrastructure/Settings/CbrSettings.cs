namespace CloudMining.Infrastructure.Settings;

public class CbrSettings
{
    public static readonly string SectionName = "Cbr";
    public string BaseUrl { get; set; }
    public string UsdCode { get; set; }
    public CbrEndpoints Endpoints { get; set; }
}

public class CbrEndpoints
{
    public string GetHistoricalPriceDataUrl { get; set; }
    public string GetDailyPriceDataUrl { get; set; }
}