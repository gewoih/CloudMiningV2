using CloudMining.Domain.Models.Identity;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Payments.Admin;
using CloudMining.Interfaces.DTO.Users;

namespace CloudMining.Application.Mappings;

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