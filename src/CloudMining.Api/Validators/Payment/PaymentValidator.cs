using CloudMining.Interfaces.DTO.Payments;
using FluentValidation;

namespace CloudMining.Api.Validators.Payment;

public class PaymentValidator : AbstractValidator<CreatePaymentDto>
{
    public PaymentValidator()
    {
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(100).WithMessage("Сумма платежа не может быть меньше 100");
        RuleFor(x => x.Date)
            .GreaterThanOrEqualTo(new DateTime(2021, 6, 1)).WithMessage("Дата платежа не может быть раньше создания проекта");
    }
}