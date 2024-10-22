using CloudMining.Domain.Models.Payments;
using CloudMining.Interfaces.DTO.Payments.Deposits;
using CloudMining.Interfaces.DTO.Users;

namespace CloudMining.Interfaces.Interfaces;

public interface IDepositService
{
	Task<List<Deposit>> GetUserDeposits(Guid userId);
	Task<Deposit> AddDepositAndRecalculateShares(DepositDto depositDto);
	Task<Dictionary<Guid, List<DepositDto>>> GetDepositsPerUserAsync(IEnumerable<UserDto> users);
}