using CloudMining.Application.Models.Shares;
using CloudMining.Domain.Models;

namespace CloudMining.Application.Services.Shares
{
	public interface IShareService
	{
		Task<decimal> GetUserShareAsync(Guid userId);
		Task<List<UserShare>> GetUsersSharesAsync();
		Task UpdateUsersSharesAsync(Deposit deposit);
		Task<List<PaymentShare>> CreatePaymentShares(decimal amount, Currency currency, DateTime date);
	}
}
