﻿using CloudMining.Application.Mappings;
using CloudMining.Application.Services;
using CloudMining.Domain.Models.Identity;
using CloudMining.Domain.Models.Payments;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Domain.Models.Purchases;
using CloudMining.Domain.Models.UserSettings;
using CloudMining.Infrastructure.Binance;
using CloudMining.Infrastructure.CentralBankRussia;
using CloudMining.Infrastructure.Database;
using CloudMining.Infrastructure.Emcd;
using CloudMining.Infrastructure.Telegram;
using CloudMining.Interfaces.DTO.Members;
using CloudMining.Interfaces.DTO.NotificationSettings;
using CloudMining.Interfaces.DTO.Payments;
using CloudMining.Interfaces.DTO.Payments.Admin;
using CloudMining.Interfaces.DTO.Payments.User;
using CloudMining.Interfaces.DTO.Purchases;
using CloudMining.Interfaces.DTO.Users;
using CloudMining.Interfaces.Interfaces;
using Telegram.Bot;
using DepositDto = CloudMining.Interfaces.DTO.Payments.Deposits.DepositDto;

namespace CloudMining.Api.Startup;

public static class ServicesSetup
{
	public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<DatabaseInitializer>();
		
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
		services.AddScoped<HoldCalculationStrategy>();
		services.AddScoped<ReceiveAndSellCalculationStrategy>();
		services.AddScoped<IMarketDataLoaderStrategyFactory, MarketDataLoaderStrategyFactory>();
		services.AddScoped<IStatisticsCalculationStrategyFactory, StatisticsCalculationStrategyFactory>();
		services.AddScoped<IStatisticsService, StatisticsService>();
		services.AddScoped<IStatisticsHelper, StatisticsHelper>();
		services.AddScoped<IPurchaseService, PurchaseService>();

		services.AddScoped<IMapper<ShareablePayment, UserPaymentDto>, UserPaymentMapper>();
		services.AddScoped<IMapper<NotificationSettings, NotificationSettingsDto>, NotificationSettingsMapper>();
		services.AddScoped<IMapper<User, MemberDto>, MemberMapper>();
		services.AddSingleton<IMapper<ShareablePayment, AdminPaymentDto>, AdminPaymentMapper>();
		services.AddSingleton<IMapper<PaymentShare, PaymentShareDto>, PaymentShareMapper>();
		services.AddSingleton<IMapper<Deposit, DepositDto>, DepositMapper>();
		services.AddSingleton<IMapper<ShareablePayment, CreatePaymentDto>, ShareablePaymentMapper>();
		services.AddSingleton<IMapper<User, RegisterDto>, UserRegistrationMapper>();
		services.AddSingleton<IMapper<Purchase, PurchaseDto>, PurchaseMapper>();

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