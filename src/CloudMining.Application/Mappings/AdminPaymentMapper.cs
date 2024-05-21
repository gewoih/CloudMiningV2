using CloudMining.Domain.Models;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.DTO.Payments.Admin;

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
            Currency = new CurrencyDto()
            {
                Code = model.Currency.Code,
                Precision = model.Currency.Precision,
                ShortName = model.Currency.ShortName
            },
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