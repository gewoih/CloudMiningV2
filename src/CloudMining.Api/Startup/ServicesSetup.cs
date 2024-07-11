using CloudMining.Application.Mappings;
using CloudMining.Application.Services;
using CloudMining.Domain.Models.Identity;
using CloudMining.Domain.Models.Payments;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Domain.Models.UserSettings;
using CloudMining.Infrastructure.Binance;
using CloudMining.Infrastructure.CentralBankRussia;
using CloudMining.Infrastructure.Emcd;
using CloudMining.Infrastructure.Telegram;
using CloudMining.Interfaces.DTO.Members;
using CloudMining.Interfaces.DTO.NotificationSettings;
using CloudMining.Interfaces.DTO.Payments.Admin;
using CloudMining.Interfaces.DTO.Payments.Deposits;
using CloudMining.Interfaces.DTO.Payments.User;
using CloudMining.Interfaces.Interfaces;
using Telegram.Bot;
using DepositDto = CloudMining.Interfaces.DTO.Payments.Deposits.DepositDto;

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
		services.AddScoped<IStatisticsService, StatisticsService>();

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