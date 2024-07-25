using CloudMining.Common.Mappers;
using CloudMining.Common.Models.Identity;
using CloudMining.Common.Models.Payments;
using CloudMining.Common.Models.Payments.Shareable;
using CloudMining.Common.Models.UserSettings;
using CloudMining.Common.Services.JWT;
using CloudMining.Common.Services.Storage;
using Modules.Currencies.Application.Services;
using Modules.Currencies.Contracts.Interfaces;
using Modules.MarketData.Application.Services;
using Modules.MarketData.Contracts.Interfaces;
using Modules.MarketData.Infrastructure.Binance;
using Modules.MarketData.Infrastructure.CentralBankRussia;
using Modules.Notifications.Application.Mappers;
using Modules.Notifications.Application.Services;
using Modules.Notifications.Contracts.DTO;
using Modules.Notifications.Contracts.Interfaces;
using Modules.Payments.Application.Mappers;
using Modules.Payments.Application.Services;
using Modules.Payments.Contracts.DTO.Admin;
using Modules.Payments.Contracts.DTO.Deposits;
using Modules.Payments.Contracts.DTO.User;
using Modules.Payments.Contracts.Interfaces;
using Modules.Payments.Infrastructure.Emcd;
using Modules.Users.Application.Services;
using Modules.Users.Contracts.Interfaces;
using Modules.Users.Infrastructure.Telegram;
using Telegram.Bot;

namespace CloudMining.Api.Startup;

public static class ServicesSetup
{
	public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<IUserManagementService, UserManagementService>();
		services.AddScoped<IAuthService, AuthService>();
		services.AddScoped<ICurrentUserService, CurrentUserService>();

		services.AddScoped<JwtService>();

		services.AddScoped<ICurrencyService, CurrencyService>();
		services.AddScoped<IShareService, ShareService>();
		services.AddScoped<IShareablePaymentService, ShareablePaymentService>();
		services.AddScoped<IDepositService, DepositService>();

		services.AddScoped<IStorageService, LocalStorageService>();

		services.AddScoped<INotificationManagementService, NotificationManagementService>();
		services.AddScoped<INotificationSettingsService, NotificationSettingsService>();
		services.AddScoped<INotificationService, TelegramNotificationService>();
		services.AddScoped<IMarketDataService, MarketDataService>();
		services.AddScoped<CryptoMarketDataLoaderStrategy>();
		services.AddScoped<FiatMarketDataLoaderStrategy>();
		services.AddScoped<IMarketDataLoaderStrategyFactory, MarketDataLoaderStrategyFactory>();

		services.AddScoped<IMapper<ShareablePayment, UserPaymentDto>, UserPaymentMapper>();
		services.AddScoped<IMapper<NotificationSettings, NotificationSettingsDto>, NotificationSettingsMapper>();
		services.AddScoped<IMapper<User, MemberDto>, MemberMapper>();
		services.AddSingleton<IMapper<ShareablePayment, AdminPaymentDto>, AdminPaymentMapper>();
		services.AddSingleton<IMapper<PaymentShare, PaymentShareDto>, PaymentShareMapper>();
		services.AddSingleton<IMapper<Deposit, DepositDto>, DepositMapper>();

		services.AddHttpClient<EmcdApiClient>();
		services.AddHostedService<PayoutsLoaderService>();
		
		services.AddHttpClient<BinanceApiClient>();
		services.AddHttpClient<CentralBankRussiaApiClient>();
		services.AddHostedService<MarketDataLoaderService>();
		

		var telegramBotApiKey = configuration["Telegram:ApiKey"];
		services.AddSingleton<ITelegramBotClient>(sp => new TelegramBotClient(telegramBotApiKey));
		services.AddHostedService<TelegramService>();

		return services;
	}
}