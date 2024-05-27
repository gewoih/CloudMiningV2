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
			Currency = new CurrencyDto(model.Currency.ShortName, model.Currency.Code, model.Currency.Precision),
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
			Amount = dto.Amount
		};
	}
}