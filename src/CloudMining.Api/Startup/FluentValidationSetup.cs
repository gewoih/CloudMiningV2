using FluentValidation;
using FluentValidation.AspNetCore;
using Modules.Payments.Api.Validators;
using Modules.Payments.Contracts.DTO;
using Modules.Payments.Contracts.DTO.Deposits;
using Modules.Users.Api.Validators;
using Modules.Users.Contracts.DTO;

namespace CloudMining.Api.Startup;

public static class FluentValidationSetup
{
	public static IServiceCollection ConfigureFluentValidation(this IServiceCollection services)
	{
		services.AddFluentValidationAutoValidation();
		services.AddScoped<IValidator<LoginDto>, LoginValidator>();
		services.AddScoped<IValidator<RegisterDto>, RegisterValidator>();
		services.AddScoped<IValidator<CreatePaymentDto>, PaymentValidator>();
		services.AddScoped<IValidator<DepositDto>, DepositValidator>();

		return services;
	}
}