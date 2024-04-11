using CloudMining.Application.Models.Shares;
using CloudMining.Domain.Models;

namespace CloudMining.Application.Services.ShareChanges
{
	public interface IUserShareService
	{
		Task<decimal> GetUserShareAsync(Guid userId);
		Task<List<UserShare>> GetUsersSharesAsync();
		Task UpdateUsersSharesAsync(Deposit deposit);
	}
}
