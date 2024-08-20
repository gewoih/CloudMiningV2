using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Infrastructure.Database;
using CloudMining.Infrastructure.Settings;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services;

public sealed class MarketDataService : IMarketDataService
{
    private readonly CloudMiningContext _context;

    public MarketDataService(CloudMiningContext context)
    {
        _context = context;
    }

    public async Task SaveMarketDataAsync(List<MarketData> marketData)
    {
        var existingCombinations = await _context.MarketData
            .GroupBy(data => new { data.From, data.To })
            .Select(group => new { group.Key.From, group.Key.To, MaxDate = group.Max(x => x.Date)})
            .ToListAsync();

        var existingCombinationsHashSet = existingCombinations
            .Select(data => (data.From, data.To, data.MaxDate))
            .ToHashSet();

        foreach (var data in marketData)
        {
            var combo = (data.From, data.To, data.Date);
            if (!existingCombinationsHashSet.Contains(combo))
                await _context.MarketData.AddAsync(data);
        }

        await _context.SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public async Task<DateTime?> GetLastMarketDataDateAsync(CurrencyCode fromCurrency, CurrencyCode toCurrency)
    {
        var lastMarketDataDate = await _context.MarketData
            .Where(marketData => marketData.From == fromCurrency && marketData.To == toCurrency)
            .MaxAsync(marketData => (DateTime?)marketData.Date);

        return lastMarketDataDate;
    }

    public async Task<Dictionary<CurrencyPair, MarketData?>> GetLatestMarketDataForCurrenciesAsync(IEnumerable<CurrencyPair> currencyPairs)
    {
        var result = new Dictionary<CurrencyPair, MarketData?>();
        
        foreach (var currencyPair in currencyPairs)
        {
            var latestMarketData = await _context.MarketData
                .Where(marketData => marketData.From == currencyPair.From && marketData.To == currencyPair.To)
                .OrderByDescending(marketData => marketData.Date)
                .FirstOrDefaultAsync();
            
            result[currencyPair] = latestMarketData;
        }

        return result;
    }

    public async Task<decimal> GetLastUsdToRubRate()
    {
        var usdToRubRate = await _context.MarketData
            .Where(marketData => marketData.From == CurrencyCode.USD && marketData.To == CurrencyCode.RUB)
            .OrderByDescending(marketData => marketData.Date)
            .Select(marketData => marketData.Price)
            .FirstOrDefaultAsync();
        return usdToRubRate;
    }
}