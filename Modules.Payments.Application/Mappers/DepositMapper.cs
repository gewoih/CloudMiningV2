using CloudMining.Common.Mappers;
using Modules.Payments.Contracts.DTO.Deposits;
using Modules.Payments.Domain.Models;

namespace Modules.Payments.Application.Mappers;

public class DepositMapper : IMapper<Deposit, DepositDto>
{
	public DepositDto ToDto(Deposit model)
	{
		return new DepositDto(model.UserId, model.Amount, model.Date);
	}

	public Deposit ToDomain(DepositDto dto)
	{
		return new Deposit
		{
			UserId = dto.UserId,
			Amount = dto.Amount,
			Date = dto.Date
		};
	}
}