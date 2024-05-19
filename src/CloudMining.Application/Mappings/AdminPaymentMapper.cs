using CloudMining.Application.DTO.Payments.Admin;
using CloudMining.Domain.Models;
using CloudMining.Domain.Models.Payments.Shareable;

namespace CloudMining.Application.Mappings;

public class AdminPaymentMapper : IMapper<ShareablePayment, AdminPaymentDto>
{
    public AdminPaymentDto ToDto(ShareablePayment model)
    {
        return new AdminPaymentDto
        {
            Id = model.Id,
            Caption = model.Caption,
            Date = model.Date,
            Amount = model.Amount,
            IsCompleted = model.IsCompleted
        };
    }

    public ShareablePayment ToDomain(AdminPaymentDto dto)
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