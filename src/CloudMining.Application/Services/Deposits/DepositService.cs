using CloudMining.Application.DTO.Payments.Deposits;
using CloudMining.Application.Models.Payments.Deposits;
using CloudMining.Application.Models.Shares;
using CloudMining.Application.Services.Shares;
using CloudMining.Domain.Models;
using CloudMining.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services.Deposits
{
	public sealed class DepositService : IDepositService
	{
		private readonly CloudMiningContext _context;
		private readonly IShareService _shareService;

		public DepositService(CloudMiningContext context, IShareService shareService)
		{
			_context = context;
			_shareService = shareService;
		}

		public async Task<Deposit> AddDepositAndRecalculateShares(CreateDepositDto depositDto)
		{
			var deposit = new Deposit
			{
				UserId = depositDto.UserId,
				Amount = depositDto.Amount,
				CurrencyId = depositDto.CurrencyId,
				CreatedDate = depositDto.Date
			};

			await using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var usersDeposits = await GetUsersDeposits();
				usersDeposits[depositDto.UserId] += depositDto.Amount;

				var newShareChanges = await _shareService.GetUpdatedUsersSharesAsync(usersDeposits, depositDto.Date);
				deposit.ShareChanges = newShareChanges;

				await _context.Deposits.AddAsync(deposit);
				await _context.SaveChangesAsync();

				await transaction.CommitAsync();
			}
			catch
			{
				await transaction.RollbackAsync();
			}

			return deposit;
		}

		public async Task<Dictionary<Guid, decimal>> GetUsersDeposits()
		{
			var usersDeposits = await _context.Users
				.Include(user => user.Deposits)
				.ToDictionaryAsync(group => group.Id, group => group.Deposits.Sum(deposit => deposit.Amount))
				.ConfigureAwait(false);

			return usersDeposits;
		}
	}
}
