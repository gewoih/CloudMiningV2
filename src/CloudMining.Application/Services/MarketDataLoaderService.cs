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
    private readonly BinanceApiClient _binanceApiClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly List<CurrencyPairs> _сurrencyPairs;


    public MarketDataLoaderService(BinanceApiClient binanceApiClient,
        IOptions<MarketDataLoaderSettings> settings,
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _delay = settings.Value.Delay;
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
        var marketDataList = new List<MarketData>();
        var endDate = DateTime.UtcNow;

        foreach (var currencyPair in _сurrencyPairs)
        {
            var foundedDate = await context.MarketData
                .Where(marketData => 
                    marketData.From == currencyPair.From && marketData.To == currencyPair.To)
                .MaxAsync(marketData => (DateTime?)marketData.Date);

            var startDate = foundedDate?.AddHours(1) ?? new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            if ((endDate - startDate).TotalHours < 1)
            {
                break;
            }

            while (startDate < endDate)
            {
                var priceData = await _binanceApiClient.GetPriceData(currencyPair.From, currencyPair.To,
                    fromDate: startDate, limit: 1000);
                if (priceData.Count == 0)
                {
                    break;
                }

                marketDataList.AddRange(priceData.Select(data => new MarketData
                {
                    From = currencyPair.From,
                    To = currencyPair.To,
                    Price = data.Price,
                    Date = data.Date
                }));

                startDate = priceData.Max(data => data.Date).AddSeconds(1);
            }
        }

        if (marketDataList.Count != 0)
        {
            context.MarketData.AddRange(marketDataList);
            await context.SaveChangesAsync();
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
}