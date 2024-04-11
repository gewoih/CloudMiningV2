using CloudMining.Domain.Models;
using CloudMining.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services.Shares
{
	public sealed class SharesChangesService : ISharesChangesService
	{
		private readonly CloudMiningContext _context;

		public SharesChangesService(CloudMiningContext context)
		{
			_context = context;
		}

		public async Task<decimal> GetUserShareAsync(Guid userId)
		{
			var userShare = await _context.ShareChanges
				.OrderByDescending(shareChange => shareChange.CreatedDate)
				.Where(shareChange => shareChange.UserId == userId)
				.Select(shareChange => shareChange.After)
				.FirstOrDefaultAsync()
				.ConfigureAwait(false);

			return userShare;
		}

		public async Task<List<KeyValuePair<Guid, decimal>>> GetUsersSharesAsync()
		{
			var usersShares = await _context.ShareChanges
				.GroupBy(shareChange => shareChange.UserId)
				.Select(group => group.OrderByDescending(group => group.CreatedDate).FirstOrDefault())
				.Select(shareChange => new KeyValuePair<Guid, decimal>(shareChange.UserId, shareChange.After))
				.ToListAsync()
				.ConfigureAwait(false);

			return usersShares;
		}

		public async Task CreateSharesChangesAsync(Deposit deposit)
		{
			//TODO: Нужно узнать сумму депозитов по всем юзерам чтобы рассчитать новую долю
			var currentShares = await GetUsersSharesAsync();
			var newSharesChanges = currentShares.Select(shareChange => 
				new ShareChange
				{
					UserId = shareChange.Key, 
					Before = shareChange.Value, 
					After = 0, 
					CreatedDate = deposit.CreatedDate
				});

			await _context.ShareChanges.AddRangeAsync(newSharesChanges).ConfigureAwait(false);
			await _context.SaveChangesAsync().ConfigureAwait(false);
		}
	}
}
