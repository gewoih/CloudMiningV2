using CloudMining.Domain.Models.Payments;
using CloudMining.Interfaces.DTO.Payments.Deposits;

namespace CloudMining.Interfaces.Interfaces;

public interface IDepositService
{
	Task<List<Deposit>> GetUserDeposits(Guid userId);
	Task<Deposit> AddDepositAndRecalculateShares(DepositDto depositDto);
}