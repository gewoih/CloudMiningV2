using FluentValidation;
using Modules.Currencies.Domain.Enums;
using Modules.Payments.Contracts.DTO;

namespace Modules.Payments.Api.Validators;

public class PaymentValidator : AbstractValidator<CreatePaymentDto>
{
	public PaymentValidator()
	{
		RuleFor(x => x.Amount)
			.GreaterThan(0)
			.WithMessage("Сумма платежа не может быть отрицательной");
		
		RuleFor(x => x.Amount)
			.GreaterThanOrEqualTo(100)
			.When(x => x.CurrencyCode == CurrencyCode.RUB)
			.WithMessage("Сумма платежа не может быть меньше 100");
		
		RuleFor(x => x.Date)
			.GreaterThanOrEqualTo(new DateTime(2021, 6, 1))
			.WithMessage("Дата платежа не может быть раньше создания проекта");
	}
}