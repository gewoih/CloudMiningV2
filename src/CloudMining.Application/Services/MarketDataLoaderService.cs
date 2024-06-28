using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Infrastructure.Binance;
using CloudMining.Infrastructure.Database;
using CloudMining.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CloudMining.Application.Services;

public sealed class MarketDataLoaderService : BackgroundService
{
    private readonly TimeSpan _delay;
    private readonly DateTime _fromDate;
    private readonly DateTime _toDate;
    private readonly BinanceApiClient _binanceApiClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly List<CurrencyPairs> _сurrencyPairs;


    public MarketDataLoaderService(BinanceApiClient binanceApiClient,
        IOptions<MarketDataLoaderSettings> settings,
        IServiceScopeFactory scopeFactory,
        DateTime fromDate,
        DateTime toDate)
    {
        _scopeFactory = scopeFactory;
        _delay = settings.Value.Delay;
        _fromDate = fromDate != default ? fromDate : new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        _toDate = toDate != default ? toDate : DateTime.UtcNow;
        _binanceApiClient = binanceApiClient;
        _сurrencyPairs = settings.Value.CurrencyPairs;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<CloudMiningContext>();

        await LoadHistoricalMarketData(context);

        while (!stoppingToken.IsCancellationRequested)
        {
            await LoadRealTimeMarketData(context);
            await Task.Delay(_delay, stoppingToken);
        }
    }

    private async Task LoadHistoricalMarketData(CloudMiningContext context)
    {

        foreach (var currencyPair in _сurrencyPairs)
        {
            var startDate = await GetStartDateForCurrencyPair(context, currencyPair);

            if (IsDataLoaded(startDate))
                break;
            
            for (; startDate < _toDate; startDate += _delay)
            {
                var priceData = await _binanceApiClient.GetPriceData(currencyPair.From, currencyPair.To,
                    fromDate: startDate, limit: 1000);
                
                if (priceData.Count == 0)
                    break;
                
                //mapper
                var marketDataList = priceData.Select(data => new MarketData
                {
                    From = currencyPair.From,
                    To = currencyPair.To,
                    Price = data.Price,
                    Date = data.Date
                }).ToList();

                context.MarketData.AddRange(marketDataList);
                await context.SaveChangesAsync();

                startDate = priceData.Max(data => data.Date);
            }
        }
    }

    private async Task LoadRealTimeMarketData(CloudMiningContext context)
    {
        var marketDataList = new List<MarketData>();
        foreach (var currencyPair in _сurrencyPairs)
        {
            var priceData = await _binanceApiClient.GetPriceData(currencyPair.From, currencyPair.To, limit: 1);
            foreach (var data in priceData)
            {
                var marketData = new MarketData()
                {
                    From = currencyPair.From,
                    To = currencyPair.To,
                    Price = data.Price,
                    Date = data.Date
                };
                marketDataList.Add(marketData);
            }
        }

        await SaveRealTimeMarketData(marketDataList, context);
    }

    private async Task SaveRealTimeMarketData(IEnumerable<MarketData> marketData, CloudMiningContext context)
    {
        var existingCombinations = await context.MarketData
            .Where(data => data.Date == context.MarketData.Max(x => x.Date))
            .Select(data => new { data.From, data.To, data.Date })
            .ToListAsync();

        var existingCombinationsHashSet = existingCombinations
            .Select(data => (data.From, data.To, data.Date))
            .ToHashSet();

        foreach (var data in marketData)
        {
            var combo = (data.From, data.To, data.Date);
            if (!existingCombinationsHashSet.Contains(combo))
            {
                context.MarketData.Add(data);
            }
        }

        await context.SaveChangesAsync()
            .ConfigureAwait(false);
    }

    private async Task<DateTime> GetStartDateForCurrencyPair(CloudMiningContext context, CurrencyPairs currencyPair)
    {
        var lastDate = await context.MarketData
            .Where(m => m.From == currencyPair.From && m.To == currencyPair.To)
            .MaxAsync(m => (DateTime?)m.Date);

        return lastDate.HasValue ? lastDate.Value + _delay : _fromDate;
    }

    private bool IsDataLoaded(DateTime startDate)
    {
        return _toDate - startDate < _delay;
    }
}