namespace Modules.MarketData.Infrastructure.Settings;

public class CentralBankRussiaSettings
{
    public static readonly string SectionName = "CentralBankRussia";
    public string BaseUrl { get; set; }
    public CentralBankRussiaEndpoints Endpoints { get; set; }
}

public class CentralBankRussiaEndpoints
{
    public string GetHistoricalPriceDataUrl { get; set; }
    public string GetDailyPriceDataUrl { get; set; }
}