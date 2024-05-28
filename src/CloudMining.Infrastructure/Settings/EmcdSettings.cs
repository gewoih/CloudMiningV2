namespace CloudMining.Infrastructure.Settings;

public class EmcdSettings
{
	public static readonly string SectionName = "Emcd";
	public string ApiKey { get; set; }
	public string BaseUrl { get; set; }
	public Endpoints Endpoints { get; set; }
	public List<string> AvailableCoins { get; set; }
}

public class Endpoints
{
	public string GetPayoutsUrl { get; set; }
}