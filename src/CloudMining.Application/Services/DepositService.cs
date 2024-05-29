using CloudMining.Application.Mappings;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Identity;
using CloudMining.Domain.Models.Payments;
using CloudMining.Infrastructure.Database;
using CloudMining.Interfaces.DTO.Payments.Deposits;
using CloudMining.Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services;

public sealed class DepositService : IDepositService
{
    private readonly CloudMiningContext _context;
    private readonly ICurrencyService _currencyService;
    private readonly IMapper<Deposit, CreateDepositDto> _depositMapper;
    private readonly IShareService _shareService;

    public DepositService(CloudMiningContext context, IShareService shareService, ICurrencyService currencyService,
        IMapper<Deposit, CreateDepositDto> depositMapper)
    {
        _context = context;
        _shareService = shareService;
        _currencyService = currencyService;
        _depositMapper = depositMapper;
    }

    public async Task<List<Deposit>> GetUserDeposits(Guid userId)
    {
        return await _context.Deposits.Where(deposit => deposit.UserId == userId).ToListAsync();
    }

    public async Task<Deposit> AddDepositAndRecalculateShares(CreateDepositDto depositDto)
    {
        var currencyId = await _currencyService.GetIdAsync(CurrencyCode.RUB);
        var deposit = _depositMapper.ToDomain(depositDto);
        deposit.CurrencyId = currencyId;

        var firstDeposit = await _context.Deposits
            .Where(firstDeposit => firstDeposit.UserId == deposit.UserId)
            .OrderBy(firstDeposit => firstDeposit.Date)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        var firstDepositDate = firstDeposit?.Date;
        var user = await _context.Users.FindAsync(deposit.UserId);
        if (user != null && (firstDepositDate == null || deposit.Date < firstDepositDate))
        {
            user.RegistrationDate = deposit.Date;
        }

        var usersDeposits = await GetUsersDeposits();
        usersDeposits[depositDto.UserId] += depositDto.Amount;

        var newShareChanges = await _shareService.GetUpdatedUsersSharesAsync(usersDeposits, depositDto.Date);
        deposit.ShareChanges = newShareChanges;

        await _context.Deposits.AddAsync(deposit);
        await _context.SaveChangesAsync();

        return deposit;
    }

    private async Task<Dictionary<Guid, decimal>> GetUsersDeposits()
    {
        var usersDeposits = await _context.Users
            .Include(user => user.Deposits)
            .ToDictionaryAsync(group => group.Id, group => group.Deposits.Sum(deposit => deposit.Amount))
            .ConfigureAwait(false);

        return usersDeposits;
    }
}