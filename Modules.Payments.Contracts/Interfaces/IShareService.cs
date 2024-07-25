using Modules.Currencies.Domain.Models;
using Modules.Payments.Contracts.DTO;
using Modules.Payments.Domain.Models;

namespace Modules.Payments.Contracts.Interfaces;

public interface IShareService
{
	Task<decimal> GetUserShareAsync(Guid userId);
	Task<List<UserShare>> GetUsersSharesAsync();
	decimal CalculateUserShare(List<ShareChange> shareChanges);
	Task<List<ShareChange>> GetUpdatedUsersSharesAsync(Dictionary<Guid, decimal> usersDeposits, DateTime newDepositDate);
	Task<List<PaymentShare>> CreatePaymentShares(decimal amount, Currency currency, DateTime date);
}