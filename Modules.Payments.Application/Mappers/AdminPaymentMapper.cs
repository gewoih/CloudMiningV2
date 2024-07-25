using CloudMining.Common.Mappers;
using CloudMining.Common.Models.Payments.Shareable;
using Modules.Currencies.Contracts.DTO;
using Modules.Payments.Contracts.DTO.Admin;

namespace Modules.Payments.Application.Mappers;

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