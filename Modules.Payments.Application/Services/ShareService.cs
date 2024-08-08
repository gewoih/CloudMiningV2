﻿using Microsoft.EntityFrameworkCore;
using Modules.Currencies.Domain.Models;
using Modules.Payments.Contracts.DTO;
using Modules.Payments.Contracts.Interfaces;
using Modules.Payments.Domain.Enums;
using Modules.Payments.Domain.Models;
using Modules.Payments.Infrastructure.Database;
using Modules.Users.Contracts.Interfaces;

namespace Modules.Payments.Application.Services;

public sealed class ShareService : IShareService
{
	private readonly PaymentsContext _context;
	private readonly IUserManagementService _userManagementService;

	public ShareService(PaymentsContext context, IUserManagementService userManagementService)
	{
		_context = context;
		_userManagementService = userManagementService;
	}

	public async Task<decimal> GetUserShareAsync(Guid userId)
	{
		var userShare = await _context.ShareChanges
			.OrderByDescending(shareChange => shareChange.Date)
			.Where(shareChange => shareChange.UserId == userId)
			.Select(shareChange => shareChange.After)
			.FirstOrDefaultAsync()
			.ConfigureAwait(false);

		return userShare;
	}

	public async Task<List<UserShare>> GetUsersSharesAsync()
	{
		//TODO: Проверить корректность, переписывался
		var users = await _userManagementService.GetUsersAsync(withShareChanges: true);
		var usersWithShares = users.Select(user =>
			new
			{
				User = user,
				LastShareChange = user.ShareChanges.MaxBy(shareChange => shareChange.CreatedDate)
			});

		var usersShares = usersWithShares.Select(u =>
			new UserShare(u.User.Id, u.LastShareChange?.After ?? 0)).ToList();

		return usersShares;
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

	public async Task<List<PaymentShare>> CreatePaymentShares(decimal amount, Currency currency, DateTime date)
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
				Date = date.ToUniversalTime(),
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
		foreach (var userShare in usersShares)
		{
			var userSharePercent = userShare.Share / 100;
			var userNetAmount = userSharePercent * amount;

			var roundedAmount = Math.Round(userNetAmount, currency.Precision, MidpointRounding.ToZero);
			usersCalculatedShares.Add(new UserCalculatedShare(userShare.UserId, roundedAmount, userShare.Share));
		}

		return usersCalculatedShares;
	}
}