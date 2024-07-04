using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Infrastructure.Binance;
using CloudMining.Infrastructure.CentralBankRussia;
using CloudMining.Infrastructure.Database;
using CloudMining.Infrastructure.Settings;
using CloudMining.Interfaces.Interfaces;
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
    private readonly CentralBankRussiaApiClient _centralBankRussiaApiClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly List<CurrencyPair> _currencyPairs;

    public MarketDataLoaderService(BinanceApiClient binanceApiClient,
        CentralBankRussiaApiClient centralBankRussiaApiClient,
        IOptions<MarketDataLoaderSettings> settings,
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _loadingDelay = settings.Value.Delay;
        _loadHistoricalDataFrom = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        _loadHistoricalDataTo = DateTime.UtcNow;
        _binanceApiClient = binanceApiClient;
        _centralBankRussiaApiClient = centralBankRussiaApiClient;
        _currencyPairs = settings.Value.CurrencyPairs;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var marketDataService = scope.ServiceProvider.GetRequiredService<IMarketDataService>();

        await LoadHistoricalMarketData(marketDataService);

        while (!stoppingToken.IsCancellationRequested)
        {
            await LoadRealTimeMarketData(marketDataService);
            await Task.Delay(_loadingDelay, stoppingToken);
        }
    }

    private async Task LoadHistoricalMarketData(IMarketDataService marketDataService)
    {
        foreach (var currencyPair in _currencyPairs)
        {
            var lastMarketDataDate = await GetLastMarketDataDate(marketDataService, currencyPair);
            var loadedMarketData = currencyPair.From != CurrencyCode.USD
                ? await LoadCryptoMarketData(currencyPair, lastMarketDataDate)
                : await LoadFiatMarketData(currencyPair, lastMarketDataDate);

            await marketDataService.SaveMarketData(loadedMarketData);
        }
    }

    private async Task LoadRealTimeMarketData(IMarketDataService marketDataService)
    {
        var marketDataList = new List<MarketData>();
        foreach (var currencyPair in _currencyPairs)
        {
            var priceData = currencyPair.From != CurrencyCode.USD
                ? await _binanceApiClient.GetMarketDataAsync(currencyPair.From, currencyPair.To, limit: 1)
                : await _centralBankRussiaApiClient.GetDailyMarketDataAsync();
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

        await marketDataService.SaveMarketData(marketDataList);
    }

    private async Task<DateTime> GetLastMarketDataDate(IMarketDataService marketDataService, CurrencyPair currencyPair)
    {
        var lastMarketDataDate = await marketDataService.GetLastMarketDataDate(currencyPair.From, currencyPair.To);
        if (lastMarketDataDate is null)
        {
            return _loadHistoricalDataFrom;
        }

        return currencyPair.From != CurrencyCode.USD
            ? lastMarketDataDate.Value.Add(_loadingDelay)
            : lastMarketDataDate.Value.AddDays(1);
    }
    
    private async Task<List<MarketData>> LoadCryptoMarketData(CurrencyPair currencyPair, DateTime lastMarketDataDate)
    {
        var loadedMarketData = new List<MarketData>();
        for (; lastMarketDataDate < _loadHistoricalDataTo; lastMarketDataDate += _loadingDelay)
        {
            var binanceMarketData = await _binanceApiClient.GetMarketDataAsync(
                fromCurrency: currencyPair.From,
                toCurrency: currencyPair.To,
                fromDate: lastMarketDataDate,
                limit: 1000);

            if (binanceMarketData.Count == 0)
                break;

            loadedMarketData.AddRange(binanceMarketData.Select(data => new MarketData
            {
                From = currencyPair.From,
                To = currencyPair.To,
                Price = data.Price,
                Date = data.Date
            }));
        
            lastMarketDataDate = binanceMarketData.Max(data => data.Date);
        }
        return loadedMarketData;
    }

    private async Task<List<MarketData>> LoadFiatMarketData(CurrencyPair currencyPair, DateTime lastMarketDataDate)
    {
        var loadedMarketData = new List<MarketData>();

            var centralBankRussiaMarketData = await _centralBankRussiaApiClient.GetHistoricalMarketDataAsync(lastMarketDataDate, _loadHistoricalDataTo);

            if (centralBankRussiaMarketData.Count == 0)
                return loadedMarketData;

            loadedMarketData.AddRange(centralBankRussiaMarketData.Select(data => new MarketData
            {
                From = currencyPair.From,
                To = currencyPair.To,
                Price = data.Price,
                Date = data.Date
            }));

        return loadedMarketData;
    }
}