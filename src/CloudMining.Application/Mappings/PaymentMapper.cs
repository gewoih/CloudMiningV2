using CloudMining.Application.DTO.Payments;
using CloudMining.Domain.Models;

namespace CloudMining.Application.Mappings;

public class PaymentMapper : IMapper<ShareablePayment, PaymentDto>
{
    public PaymentDto ToDto(ShareablePayment model)
    {
        return new PaymentDto
        {
            Id = model.Id,
            Caption = model.Caption,
            Date = model.Date,
            Amount = model.Amount,
            IsCompleted = model.IsCompleted
        };
    }

    public ShareablePayment ToDomain(PaymentDto dto)
    {
        return new ShareablePayment
        {
            Id = dto.Id,
            Caption = dto.Caption,
            Date = dto.Date,
            Amount = dto.Amount,
            IsCompleted = dto.IsCompleted
        };
    }
}