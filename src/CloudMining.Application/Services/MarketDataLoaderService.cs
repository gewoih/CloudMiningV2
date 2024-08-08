using CloudMining.Domain.Models.Currencies;
using CloudMining.Infrastructure.Settings;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CloudMining.Application.Services;

public sealed class MarketDataLoaderService : BackgroundService
{
    private readonly TimeSpan _loadingDelay;

    //TODO: Вынести параметры с датами в appsettings
    private readonly DateTime _loadHistoricalDataFrom = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private readonly DateTime _loadHistoricalDataTo = DateTime.UtcNow;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly List<CurrencyPair> _currencyPairs;

    public MarketDataLoaderService(IOptions<MarketDataLoaderSettings> settings,
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _loadingDelay = settings.Value.Delay;
        _currencyPairs = settings.Value.CurrencyPairs;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var marketDataService = scope.ServiceProvider.GetRequiredService<IMarketDataService>();

            await LoadAndSaveMarketData(marketDataService);

            await Task.Delay(_loadingDelay, stoppingToken);
        }
    }

    private async Task LoadAndSaveMarketData(IMarketDataService marketDataService)
    {
        foreach (var currencyPair in _currencyPairs)
        {
            var lastMarketDataDate =
                await marketDataService.GetLastMarketDataDateAsync(currencyPair.From, currencyPair.To) ??
                _loadHistoricalDataFrom;

            var marketData = await GetMarketDataAsync(currencyPair, lastMarketDataDate, _loadHistoricalDataTo);
            await marketDataService.SaveMarketDataAsync(marketData);
        }
    }

    private async Task<List<MarketData>> GetMarketDataAsync(CurrencyPair currencyPair,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var marketDataLoaderStrategyFactory =
            scope.ServiceProvider.GetRequiredService<IMarketDataLoaderStrategyFactory>();
        var marketDataLoaderStrategy = marketDataLoaderStrategyFactory.Create(currencyPair);
        var marketData =
            await marketDataLoaderStrategy.GetMarketDataAsync(currencyPair, fromDate, toDate);

        return marketData;
    }
}