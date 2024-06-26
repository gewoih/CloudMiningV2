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
    private readonly int _delayInMinutes;
    private readonly BinanceApiClient _binanceApiClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly List<CurrencyPairs> _сurrencyPairs;


    public MarketDataLoaderService(BinanceApiClient binanceApiClient,
        IOptions<MarketDataLoaderSettings> settings,
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _delayInMinutes = settings.Value.DelayInMinutes;
        _binanceApiClient = binanceApiClient;
        _сurrencyPairs = settings.Value.CurrencyPairs;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<CloudMiningContext>();
            await LoadRealTimeMarketData(context);

            await Task.Delay(TimeSpan.FromMinutes(_delayInMinutes), stoppingToken);
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
                existingCombinationsHashSet.Add(combo);
            }
        }

        await context.SaveChangesAsync()
            .ConfigureAwait(false);
    }
}