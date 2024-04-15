using CloudMining.Application.Models.Payments.Deposits;
using CloudMining.Application.Models.Shares;
using CloudMining.Domain.Models;

namespace CloudMining.Application.Services.Shares
{
    public interface IShareService
	{
		Task<decimal> GetUserShareAsync(Guid userId);
		Task<List<UserShare>> GetUsersSharesAsync();
		Task<List<ShareChange>> GetUpdatedUsersSharesAsync(Dictionary<Guid, decimal> usersDeposits,
			DateTime newDepositDate);
		Task<List<PaymentShare>> CreatePaymentShares(decimal amount, Currency currency, DateTime date);
	}
}
