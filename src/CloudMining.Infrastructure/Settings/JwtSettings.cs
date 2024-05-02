namespace CloudMining.Infrastructure.Settings;

public class JwtSettings
{
    public static readonly string SectionName = "Jwt";
    public string SigningKey { get; set; }
    public int LifetimeInDays { get; set; }
}