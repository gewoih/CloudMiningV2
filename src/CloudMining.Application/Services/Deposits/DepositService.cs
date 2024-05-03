using CloudMining.Application.DTO.Payments.Deposits;
using CloudMining.Application.Mappings;
using CloudMining.Application.Services.Currencies;
using CloudMining.Application.Services.Shares;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;
using CloudMining.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services.Deposits
{
	public sealed class DepositService : IDepositService
	{
		private readonly CloudMiningContext _context;
		private readonly IShareService _shareService;
		private readonly ICurrencyService _currencyService;
		private readonly IMapper<Deposit, CreateDepositDto> _depositMapper;

		public DepositService(CloudMiningContext context, IShareService shareService, ICurrencyService currencyService, IMapper<Deposit, CreateDepositDto> depositMapper)
		{
			_context = context;
			_shareService = shareService;
			_currencyService = currencyService;
			_depositMapper = depositMapper;
		}

		public async Task<Deposit> AddDepositAndRecalculateShares(CreateDepositDto depositDto)
		{
			var currencyId = await _currencyService.GetIdAsync(CurrencyCode.RUB);
			var deposit = _depositMapper.ToDomain(depositDto);
			deposit.CurrencyId = currencyId;

			var usersDeposits = await GetUsersDeposits();
			usersDeposits[depositDto.UserId] += depositDto.Amount;

			var newShareChanges = await _shareService.GetUpdatedUsersSharesAsync(usersDeposits, depositDto.Date);
			deposit.ShareChanges = newShareChanges;

			await _context.Deposits.AddAsync(deposit);
			await _context.SaveChangesAsync();

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
