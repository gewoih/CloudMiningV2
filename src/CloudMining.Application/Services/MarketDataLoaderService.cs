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
    private readonly TimeSpan _loadingDelay;
    private readonly DateTime _loadHistoricalDataFrom;
    private readonly DateTime _loadHistoricalDataTo;
    private readonly BinanceApiClient _binanceApiClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly List<CurrencyPair> _currencyPairs;

    public MarketDataLoaderService(BinanceApiClient binanceApiClient,
        IOptions<MarketDataLoaderSettings> settings,
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _loadingDelay = settings.Value.Delay;
        _loadHistoricalDataFrom = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        _loadHistoricalDataTo = DateTime.UtcNow;
        _binanceApiClient = binanceApiClient;
        _currencyPairs = settings.Value.CurrencyPairs;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<CloudMiningContext>();

        await LoadHistoricalMarketData(context);

        while (!stoppingToken.IsCancellationRequested)
        {
            await LoadRealTimeMarketData(context);
            await Task.Delay(_loadingDelay, stoppingToken);
        }
    }

    private async Task LoadHistoricalMarketData(CloudMiningContext context)
    {
        foreach (var currencyPair in _currencyPairs)
        {
            var loadedMarketData = new List<MarketData>();

            var lastMarketDataDate = await GetLastMarketDataDate(context, currencyPair);
            if (lastMarketDataDate is null)
                lastMarketDataDate = _loadHistoricalDataFrom;
            else
                lastMarketDataDate += _loadingDelay;
            
            for (; lastMarketDataDate < _loadHistoricalDataTo; lastMarketDataDate += _loadingDelay)
            {
                var binanceMarketData = await _binanceApiClient.GetMarketDataAsync(
                    fromCurrency: currencyPair.From,
                    toCurrency: currencyPair.To,
                    fromDate: lastMarketDataDate, 
                    limit: 1000);
                
                if (binanceMarketData.Count == 0)
                    break;

                var newMarketData = binanceMarketData.Select(data => new MarketData
                {
                    From = currencyPair.From,
                    To = currencyPair.To,
                    Price = data.Price,
                    Date = data.Date
                });

				loadedMarketData.AddRange(newMarketData);
                lastMarketDataDate = binanceMarketData.Max(data => data.Date);
            }

            await context.MarketData.AddRangeAsync(loadedMarketData);
            await context.SaveChangesAsync();
        }
    }

    private async Task LoadRealTimeMarketData(CloudMiningContext context)
    {
        var marketDataList = new List<MarketData>();
        foreach (var currencyPair in _currencyPairs)
        {
            var priceData = await _binanceApiClient.GetMarketDataAsync(currencyPair.From, currencyPair.To, limit: 1);
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

    private static async Task SaveRealTimeMarketData(IEnumerable<MarketData> marketData, CloudMiningContext context)
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
                await context.MarketData.AddAsync(data);
        }

        await context.SaveChangesAsync()
            .ConfigureAwait(false);
    }

    private static async Task<DateTime?> GetLastMarketDataDate(CloudMiningContext context, CurrencyPair currencyPair)
    {
        var lastMarketDataDate = await context.MarketData
            .Where(marketData => marketData.From == currencyPair.From && marketData.To == currencyPair.To)
            .MaxAsync(marketData => (DateTime?)marketData.Date);

        return lastMarketDataDate;
    }
}