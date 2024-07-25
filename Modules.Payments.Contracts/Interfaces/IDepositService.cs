using Modules.Payments.Contracts.DTO.Deposits;
using Modules.Payments.Domain.Models;

namespace Modules.Payments.Contracts.Interfaces;

public interface IDepositService
{
	Task<List<Deposit>> GetUserDeposits(Guid userId);
	Task<Deposit> AddDepositAndRecalculateShares(DepositDto depositDto);
}