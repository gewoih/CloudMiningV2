using CloudMining.Infrastructure.Settings;

namespace CloudMining.Api.Startup;

public static class SettingsSetup
{
	public static IServiceCollection ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<EmcdSettings>(configuration.GetSection(EmcdSettings.SectionName));
		services.Configure<PayoutsLoaderSettings>(configuration.GetSection(PayoutsLoaderSettings.SectionName));
		services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
		services.Configure<StorageSettings>(configuration.GetSection(StorageSettings.SectionName));
		services.Configure<BinanceSettings>(configuration.GetSection(BinanceSettings.SectionName));
		services.Configure<ProjectInformationSettings>(configuration.GetSection(ProjectInformationSettings.SectionName));
		services.Configure<CentralBankRussiaSettings>(configuration.GetSection(CentralBankRussiaSettings.SectionName));
		services.Configure<MarketDataLoaderSettings>(configuration.GetSection(MarketDataLoaderSettings.SectionName));

		return services;
	}
}