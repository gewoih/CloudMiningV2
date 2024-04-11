using CloudMining.Domain.Models;

namespace CloudMining.Application.Services.Shares
{
	public interface ISharesChangesService
	{
		Task<decimal> GetUserShareAsync(Guid userId);
		Task<List<KeyValuePair<Guid, decimal>>> GetUsersSharesAsync();
		Task CreateSharesChangesAsync(Deposit deposit);
	}
}
