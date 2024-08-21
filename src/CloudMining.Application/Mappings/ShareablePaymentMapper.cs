using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Payments;

namespace CloudMining.Application.Mappings;

public class ShareablePaymentMapper : IMapper<ShareablePayment, CreatePaymentDto>
{
    public CreatePaymentDto ToDto(ShareablePayment model)
    {
        return new CreatePaymentDto(model.Caption, model.Currency.Code, model.Type, model.Date, model.Amount);
    }

    public ShareablePayment ToDomain(CreatePaymentDto dto)
    {
        return new ShareablePayment
        {
            Amount = dto.Amount,
            Caption = dto.Caption,
            Type = dto.PaymentType,
            Date = dto.Date.ToUniversalTime()
        };
    }
}