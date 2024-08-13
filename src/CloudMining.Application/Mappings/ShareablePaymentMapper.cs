using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Payments;

namespace CloudMining.Application.Mappings;

public class ShareablePaymentMapper: IMapper<ShareablePayment, CreatePaymentDto>
{
    public CreatePaymentDto ToDto(ShareablePayment model)
    {
        throw new NotImplementedException();
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