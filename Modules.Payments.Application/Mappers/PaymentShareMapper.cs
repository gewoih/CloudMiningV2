using CloudMining.Common.Mappers;
using CloudMining.Common.Models.Identity;
using CloudMining.Common.Models.Payments.Shareable;
using Modules.Payments.Contracts.DTO.Admin;
using Modules.Payments.Contracts.DTO.User;

namespace Modules.Payments.Application.Mappers;

public class PaymentShareMapper : IMapper<PaymentShare, PaymentShareDto>
{
	public PaymentShareDto ToDto(PaymentShare model)
	{
		var user = new UserDto(model.User.Id, model.User.FirstName, model.User.LastName, model.User.Patronymic);
		return new PaymentShareDto(model.Id, user, model.Amount, model.Share, model.Status);
	}

	public PaymentShare ToDomain(PaymentShareDto dto)
	{
		return new PaymentShare
		{
			User = new User
			{
				Id = dto.User.Id,
				FirstName = dto.User.FirstName,
				LastName = dto.User.LastName,
				Patronymic = dto.User.Patronymic
			},

			Amount = dto.Amount,
			Share = dto.Share,
			Status = dto.Status
		};
	}
}