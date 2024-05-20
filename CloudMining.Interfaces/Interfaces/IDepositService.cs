using CloudMining.Contracts.DTO.Payments.Deposits;
using CloudMining.Domain.Models.Payments;

namespace CloudMining.Contracts.Interfaces
{
	public interface IDepositService
	{
		Task<Deposit> AddDepositAndRecalculateShares(CreateDepositDto depositDto);
	}
}