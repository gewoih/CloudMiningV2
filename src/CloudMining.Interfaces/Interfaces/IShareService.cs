using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Domain.Models.Shares;

namespace CloudMining.Interfaces.Interfaces;

public interface IShareService
{
	decimal CalculateUserShare(List<ShareChange> shareChanges);
	Task<List<ShareChange>> GetUpdatedUsersSharesAsync(Dictionary<Guid, decimal> usersDeposits, DateTime newDepositDate);
	Task<List<PaymentShare>> CreatePaymentShares(decimal amount, Currency currency);
}