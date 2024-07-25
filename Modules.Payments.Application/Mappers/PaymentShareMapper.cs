using CloudMining.Common.Mappers;
using Modules.Payments.Contracts.DTO.Admin;
using Modules.Payments.Contracts.DTO.User;
using Modules.Payments.Domain.Models;

namespace Modules.Payments.Application.Mappers;

public class PaymentShareMapper : IMapper<PaymentShare, PaymentShareDto>
{
	public PaymentShareDto ToDto(PaymentShare model)
	{
		return new PaymentShareDto(model.Id, model.Amount, model.Share, model.Status);
	}

	public PaymentShare ToDomain(PaymentShareDto dto)
	{
		return new PaymentShare
		{
			Amount = dto.Amount,
			Share = dto.Share,
			Status = dto.Status
		};
	}
}