using CloudMining.Common.Models.Currencies;
using CloudMining.Common.Models.Payments.Shareable;
using CloudMining.Common.Models.Shares;
using Modules.Payments.Contracts.DTO;

namespace Modules.Payments.Contracts.Interfaces;

public interface IShareService
{
	Task<decimal> GetUserShareAsync(Guid userId);
	Task<List<UserShare>> GetUsersSharesAsync();
	decimal CalculateUserShare(List<ShareChange> shareChanges);
	Task<List<ShareChange>> GetUpdatedUsersSharesAsync(Dictionary<Guid, decimal> usersDeposits, DateTime newDepositDate);
	Task<List<PaymentShare>> CreatePaymentShares(decimal amount, Currency currency, DateTime date);
}