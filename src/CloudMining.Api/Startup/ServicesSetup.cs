using CloudMining.Application.DTO.Payments.Admin;
using CloudMining.Application.DTO.Payments.Deposits;
using CloudMining.Application.DTO.Payments.User;
using CloudMining.Application.Mappings;
using CloudMining.Application.Services.Currencies;
using CloudMining.Application.Services.Deposits;
using CloudMining.Application.Services.Files;
using CloudMining.Application.Services.JWT;
using CloudMining.Application.Services.Payments;
using CloudMining.Application.Services.Payouts;
using CloudMining.Application.Services.Shares;
using CloudMining.Application.Services.Users;
using CloudMining.Domain.Models;
using CloudMining.Infrastructure.Emcd;

namespace CloudMining.Api.Startup;

public static class ServicesSetup
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<IShareService, ShareService>();
        services.AddScoped<IShareablePaymentService, ShareablePaymentService>();
        services.AddScoped<IDepositService, DepositService>();
        services.AddScoped<IStorageService, LocalStorageService>();
        services.AddScoped<JwtService>();

        services.AddSingleton<IMapper<ShareablePayment, AdminPaymentDto>, AdminPaymentMapper>();
        services.AddScoped<IMapper<ShareablePayment, UserPaymentDto>, UserPaymentMapper>();
        services.AddSingleton<IMapper<PaymentShare, PaymentShareDto>, PaymentShareMapper>();
        services.AddSingleton<IMapper<Deposit, CreateDepositDto>, DepositMapper>();

        services.AddHttpClient<EmcdApiClient>();
        //services.AddHostedService<PayoutsLoaderService>();
        
        return services;
    }
}