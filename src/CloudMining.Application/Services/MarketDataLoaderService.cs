using CloudMining.Domain.Models.Currencies;
using CloudMining.Infrastructure.Binance;
using CloudMining.Infrastructure.Database;
using CloudMining.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CloudMining.Application.Services;

public sealed class MarketDataLoaderService : BackgroundService
{
    private readonly int _delayInMinutes;
    private readonly BinanceApiClient _binanceApiClient;
    private readonly CloudMiningContext _context;
    private readonly List<CurrencyPairs> _сurrencyPairs;


    public MarketDataLoaderService(BinanceApiClient binanceApiClient,
        IOptions<MarketDataLoaderSettings> settings, CloudMiningContext context)
    {
        _context = context;
        _delayInMinutes = settings.Value.DelayInMinutes;
        _binanceApiClient = binanceApiClient;
        _сurrencyPairs = settings.Value.CurrencyPairs;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await LoadRealTimeMarketData();

            await Task.Delay(TimeSpan.FromMinutes(_delayInMinutes), stoppingToken);
        }
    }

    private async Task LoadRealTimeMarketData()
    {
        var marketDataList = new List<MarketData>();
        foreach (var currencyPair in _сurrencyPairs)
        {
           var priceData = await _binanceApiClient.GetPriceData(currencyPair.From, currencyPair.To);
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

        await SaveRealTimeMarketData(marketDataList);
    }

    private async Task SaveRealTimeMarketData(IEnumerable<MarketData> marketData)
    {
        await _context.MarketData.AddRangeAsync(marketData)
            .ConfigureAwait(false);
        
        await _context.SaveChangesAsync()
            .ConfigureAwait(false);
    }
}