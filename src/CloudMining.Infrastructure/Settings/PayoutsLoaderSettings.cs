namespace CloudMining.Infrastructure.Settings;

public class PayoutsLoaderSettings
{
    public static readonly string SectionName = "PayoutsLoader";
    
    public int DelayInMinutes { get; set; }
}