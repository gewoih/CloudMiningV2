using CloudMining.Application.Models;
using CloudMining.Application.Models.Shares;
using CloudMining.Domain.Models;
using CloudMining.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services.ShareChanges
{
	public sealed class ShareService : IShareService
	{
		private readonly CloudMiningContext _context;

		public ShareService(CloudMiningContext context)
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

		public async Task<List<UserShare>> GetUsersSharesAsync()
		{
			var usersShares = await _context.ShareChanges
				.GroupBy(shareChange => shareChange.UserId)
				.Select(group => group.OrderByDescending(group => group.CreatedDate).FirstOrDefault())
				.Select(shareChange => new UserShare(shareChange.UserId, shareChange.After))
				.ToListAsync()
				.ConfigureAwait(false);

			return usersShares;
		}

		public async Task UpdateUsersSharesAsync(Deposit deposit)
		{
			//TODO: Нужно узнать сумму депозитов по всем юзерам чтобы рассчитать новую долю
			var currentShares = await GetUsersSharesAsync();
			var newSharesChanges = currentShares.Select(userShare =>
				new ShareChange
				{
					UserId = userShare.UserId,
					Before = userShare.Share,
					After = 0,
					CreatedDate = deposit.CreatedDate
				});

			await _context.ShareChanges.AddRangeAsync(newSharesChanges).ConfigureAwait(false);
			await _context.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task<List<PaymentShare>> CreatePaymentShares(decimal amount, Currency currency, DateTime date)
		{
			var usersShares = await CalculateUsersSharesAsync(amount, currency);

			var paymentShares = new List<PaymentShare>();
			foreach (var userShare in usersShares)
			{
				var paymentShare = new PaymentShare
				{
					UserId = userShare.UserId,
					Amount = userShare.Amount,
					Share = userShare.Share,
					CreatedDate = date
				};

				paymentShares.Add(paymentShare);
			}

			return paymentShares;
		}

		private async Task<List<UserCalculatedShare>> CalculateUsersSharesAsync(decimal amount, Currency currency)
		{
			var usersShares = await GetUsersSharesAsync();

			var usersCalculatedShares = new List<UserCalculatedShare>();
			foreach (var userShare in usersShares)
			{
				var calculatedAmount =
					Math.Round(userShare.Share * amount, currency.Precision, MidpointRounding.ToZero);

				usersCalculatedShares.Add(new UserCalculatedShare(userShare.UserId, calculatedAmount, userShare.Share));
			}

			return usersCalculatedShares;
		}
	}
}
