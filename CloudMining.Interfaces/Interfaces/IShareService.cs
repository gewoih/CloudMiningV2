using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Domain.Models.Shares;
using CloudMining.Interfaces.DTO;

namespace CloudMining.Interfaces.Interfaces;

public interface IShareService
{
	Task<decimal> GetUserShareAsync(Guid userId);
	Task<List<UserShare>> GetUsersSharesAsync();

	Task<List<ShareChange>>
		GetUpdatedUsersSharesAsync(Dictionary<Guid, decimal> usersDeposits, DateTime newDepositDate);

	Task<List<PaymentShare>> CreatePaymentShares(decimal amount, Currency currency, DateTime date);
}