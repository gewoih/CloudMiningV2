using CloudMining.Common.Models.Payments;
using Modules.Payments.Contracts.DTO.Deposits;

namespace Modules.Payments.Contracts.Interfaces;

public interface IDepositService
{
	Task<List<Deposit>> GetUserDeposits(Guid userId);
	Task<Deposit> AddDepositAndRecalculateShares(DepositDto depositDto);
}