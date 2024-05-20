using CloudMining.Interfaces.DTO.Payments.Deposits;
using FluentValidation;

namespace CloudMining.Api.Validators.Deposit;

public class DepositValidator : AbstractValidator<CreateDepositDto>
{
    public DepositValidator()
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("Депозит должен принадлежать существующему пользователю");

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(100).WithMessage("Сумма депозита не может быть меньше 100");
    }
}