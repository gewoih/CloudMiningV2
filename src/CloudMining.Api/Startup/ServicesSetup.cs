using CloudMining.Application.Mappings;
using CloudMining.Application.Services;
using CloudMining.Domain.Models;
using CloudMining.Domain.Models.Payments;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Domain.Models.UserSettings;
using CloudMining.Infrastructure.Emcd;
using CloudMining.Infrastructure.Telegram;
using CloudMining.Interfaces.DTO.NotificationSettings;
using CloudMining.Interfaces.DTO.Payments.Admin;
using CloudMining.Interfaces.DTO.Payments.Deposits;
using CloudMining.Interfaces.DTO.Payments.User;
using CloudMining.Interfaces.Interfaces;
using Telegram.Bot;

namespace CloudMining.Api.Startup;

public static class ServicesSetup
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<IShareService, ShareService>();
        services.AddScoped<IShareablePaymentService, ShareablePaymentService>();
        services.AddScoped<IDepositService, DepositService>();
        services.AddScoped<IStorageService, LocalStorageService>();
        services.AddScoped<INotificationSettingsService, NotificationSettingsService>();
        services.AddScoped<JwtService>();

        services.AddScoped<INotificationService, TelegramNotificationService>();

        services.AddSingleton<IMapper<ShareablePayment, AdminPaymentDto>, AdminPaymentMapper>();
        services.AddScoped<IMapper<ShareablePayment, UserPaymentDto>, UserPaymentMapper>();
        services.AddScoped<IMapper<NotificationSettings, NotificationSettingsDto>, NotificationSettingsMapper>();
        services.AddSingleton<IMapper<PaymentShare, PaymentShareDto>, PaymentShareMapper>();
        services.AddSingleton<IMapper<Deposit, CreateDepositDto>, DepositMapper>();

        services.AddHttpClient<EmcdApiClient>();
        //services.AddHostedService<PayoutsLoaderService>();

        var telegramBotApiKey = configuration["Telegram:ApiKey"];
        services.AddSingleton<ITelegramBotClient>(sp => new TelegramBotClient(telegramBotApiKey));
        services.AddHostedService<TelegramService>();
        
        return services;
    }
}