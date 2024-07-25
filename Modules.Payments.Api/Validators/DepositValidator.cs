using FluentValidation;
using Modules.Payments.Contracts.DTO.Deposits;

namespace Modules.Payments.Api.Validators;

public class DepositValidator : AbstractValidator<DepositDto>
{
	public DepositValidator()
	{
		RuleFor(x => x.UserId)
			.NotEqual(Guid.Empty).WithMessage("Депозит должен принадлежать существующему пользователю");

		RuleFor(x => x.Amount)
			.GreaterThanOrEqualTo(100).WithMessage("Сумма депозита не может быть меньше 100");
	}
}