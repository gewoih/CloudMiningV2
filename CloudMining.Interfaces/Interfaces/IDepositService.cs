using CloudMining.Domain.Models.Payments;
using CloudMining.Interfaces.DTO.Payments.Deposits;

namespace CloudMining.Interfaces.Interfaces
{
	public interface IDepositService
	{
		Task<Deposit> AddDepositAndRecalculateShares(CreateDepositDto depositDto);
	}
}