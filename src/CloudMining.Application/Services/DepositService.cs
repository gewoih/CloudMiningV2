﻿using CloudMining.Application.Mappings;
using CloudMining.Domain.Enums;
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
    private readonly IMapper<Deposit, DepositDto> _depositMapper;
    private readonly IShareService _shareService;

    public DepositService(CloudMiningContext context, IShareService shareService, ICurrencyService currencyService,
        IMapper<Deposit, DepositDto> depositMapper)
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

    public async Task<Deposit> AddDepositAndRecalculateShares(DepositDto depositDto)
    {
        var currencyId = await _currencyService.GetIdAsync(CurrencyCode.RUB);
        var newDeposit = _depositMapper.ToDomain(depositDto);
        newDeposit.CurrencyId = currencyId;
        
        var usersDeposits = await GetUsersDeposits();
        usersDeposits[depositDto.UserId] += depositDto.Amount;

        var newShareChanges = await _shareService.GetUpdatedUsersSharesAsync(usersDeposits, depositDto.Date);
        newDeposit.ShareChanges = newShareChanges;

        await _context.Deposits.AddAsync(newDeposit);
        await _context.SaveChangesAsync();

        return newDeposit;
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