﻿using CloudMining.Infrastructure.Settings;

namespace CloudMining.Api.Startup;

public static class SettingsSetup
{
    public static IServiceCollection ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmcdSettings>(configuration.GetSection(EmcdSettings.SectionName));
        services.Configure<PayoutsLoaderSettings>(configuration.GetSection(PayoutsLoaderSettings.SectionName));
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<StorageSettings>(configuration.GetSection(StorageSettings.SectionName));
        
        return services;
    }
}