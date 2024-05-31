using CloudMining.Domain.Models.Payments;
using CloudMining.Interfaces.DTO.Payments.Deposits;

namespace CloudMining.Application.Mappings;

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