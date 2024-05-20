using CloudMining.Api.Validators.Deposit;
using CloudMining.Api.Validators.Payment;
using CloudMining.Api.Validators.User;
using CloudMining.Contracts.DTO.Payments;
using CloudMining.Contracts.DTO.Payments.Deposits;
using CloudMining.Contracts.DTO.Users;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace CloudMining.Api.Startup;

public static class FluentValidationSetup
{
    public static IServiceCollection ConfigureFluentValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddScoped<IValidator<LoginDto>, LoginValidator>();
        services.AddScoped<IValidator<RegisterDto>, RegisterValidator>();
        services.AddScoped<IValidator<CreatePaymentDto>, PaymentValidator>();
        services.AddScoped<IValidator<CreateDepositDto>, DepositValidator>();

        return services;
    }
}