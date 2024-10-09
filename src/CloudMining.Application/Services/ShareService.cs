using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Identity;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Domain.Models.Shares;
using CloudMining.Infrastructure.Database;
using CloudMining.Interfaces.DTO;
using CloudMining.Interfaces.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services;

public sealed class ShareService : IShareService
{
	private readonly CloudMiningContext _context;
	private readonly UserManager<User> _userManager;
	private readonly RoleManager<Role> _roleManager;

	public ShareService(CloudMiningContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
	{
		_context = context;
		_userManager = userManager;
		_roleManager = roleManager;
	}

	public decimal CalculateUserShare(List<ShareChange> shareChanges)
	{
		if (shareChanges.Count == 0)
			return 0;
		
		var userShare = shareChanges
			.OrderByDescending(shareChange => shareChange.Date)
			.First()
			.After;

		return userShare;
	}

	public async Task<List<ShareChange>> GetUpdatedUsersSharesAsync(Dictionary<Guid, decimal> usersDeposits,
		DateTime newDepositDate)
	{
		var totalDepositsAmount = usersDeposits.Sum(userDeposits => userDeposits.Value);

		var currentShares = await GetUsersSharesAsync();
		var sharesChanges = new List<ShareChange>();
		foreach (var userShare in currentShares)
		{
			var userTotalDeposit = usersDeposits[userShare.UserId];
			var newShare = 0m;
			if (userTotalDeposit != 0)
				newShare = userTotalDeposit / totalDepositsAmount * 100;

			if (newShare == userShare.Share)
				continue;

			var newShareChange = new ShareChange
			{
				UserId = userShare.UserId,
				Before = userShare.Share,
				After = newShare,
				Date = newDepositDate
			};

			sharesChanges.Add(newShareChange);
		}

		return sharesChanges;
	}

	public async Task<List<PaymentShare>> CreatePaymentShares(decimal amount, Currency currency)
	{
		IEnumerable<UserCalculatedShare> usersShares = await CalculateUsersSharesAsync(amount, currency);
		usersShares = usersShares.Where(userShare => userShare.Amount != 0);

		var paymentShares = new List<PaymentShare>();
		foreach (var userShare in usersShares)
		{
			var paymentShare = new PaymentShare
			{
				UserId = userShare.UserId,
				Amount = userShare.Amount,
				Share = userShare.Share,
				Status = ShareStatus.Created
			};

			paymentShares.Add(paymentShare);
		}

		return paymentShares;
	}

	private async Task<List<UserCalculatedShare>> CalculateUsersSharesAsync(decimal amount, Currency currency)
	{
		var usersShares = await GetUsersSharesAsync();

		var usersCalculatedShares = new List<UserCalculatedShare>();
		var totalUsersCommissions = usersShares.Sum(share => share.CommissionPercent);
		var amountAfterCommissions = amount - amount * totalUsersCommissions;
		foreach (var userShare in usersShares)
		{
			var userCommissionPercent = userShare.CommissionPercent;
			var userSharePercent = userShare.Share / 100;
			var userCommission = amount * userCommissionPercent;
			var userNetAmount = amountAfterCommissions * userSharePercent;
			var userTotalNetAmount = userNetAmount + userCommission;

			var roundedAmount = Math.Round(userTotalNetAmount, currency.Precision, MidpointRounding.ToZero);
			usersCalculatedShares.Add(new UserCalculatedShare(userShare.UserId, roundedAmount, userShare.Share));
		}

		return usersCalculatedShares;
	}
	
	private async Task<List<UserShare>> GetUsersSharesAsync()
	{
		var usersWithShares = await _context.Users
			.Select(user => new
			{
				User = user,
				LastShareChange = user.ShareChanges
					.OrderByDescending(shareChange => shareChange.CreatedDate)
					.FirstOrDefault()
			})
			.ToListAsync()
			.ConfigureAwait(false);

		var usersShares = new List<UserShare>();
		foreach (var userWithShare in usersWithShares)
		{
			var userRole = (await _userManager.GetRolesAsync(userWithShare.User)).FirstOrDefault();
			var userCommissionPercent = 0m;
			if (userRole is not null)
			{
				var role = await _roleManager.FindByNameAsync(userRole);
				userCommissionPercent = role.CommissionPercent;
			}
			
			usersShares.Add(new UserShare(userWithShare.User.Id, userWithShare.LastShareChange?.After ?? 0, userCommissionPercent));
		}

		return usersShares;
	}
}