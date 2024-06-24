using CloudMining.Infrastructure.Binance;
using CloudMining.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CloudMining.Application.Services;

public sealed class MarketDataLoaderService : BackgroundService
{
    private readonly int _delayInMinutes;
    private readonly BinanceApiClient _binanceApiClient;
    private readonly IServiceScopeFactory _scopeFactory;


    public MarketDataLoaderService(BinanceApiClient binanceApiClient,
        IServiceScopeFactory scopeFactory,
        IOptions<MarketDataLoaderSettings> settings)
    {
        _delayInMinutes = settings.Value.DelayInMinutes;
        _binanceApiClient = binanceApiClient;
        _scopeFactory = scopeFactory;
    }
}