using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Infrastructure.Binance;
using CloudMining.Infrastructure.CentralBankRussia;
using CloudMining.Infrastructure.Settings;
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
	private readonly BinanceApiClient _binanceApiClient;
	private readonly CentralBankRussiaApiClient _centralBankRussiaApiClient;
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly List<CurrencyPair> _currencyPairs;
	private readonly IMarketDataLoaderStrategyFactory _marketDataLoaderStrategyFactory;

	public MarketDataLoaderService(BinanceApiClient binanceApiClient,
		CentralBankRussiaApiClient centralBankRussiaApiClient,
		IOptions<MarketDataLoaderSettings> settings,
		IServiceScopeFactory scopeFactory,
		IMarketDataLoaderStrategyFactory marketDataLoaderStrategyFactory)
	{
		_scopeFactory = scopeFactory;
		_marketDataLoaderStrategyFactory = marketDataLoaderStrategyFactory;
		_loadingDelay = settings.Value.Delay;
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
			var loadDataFrom =
				await marketDataService.GetLastMarketDataDateAsync(currencyPair.From, currencyPair.To) ??
				_loadHistoricalDataFrom;

			var marketDataLoaderStrategy = _marketDataLoaderStrategyFactory.Create(currencyPair);
			var marketData =
				await marketDataLoaderStrategy.GetMarketDataAsync(currencyPair, loadDataFrom,
					_loadHistoricalDataTo);

			await marketDataService.SaveMarketDataAsync(marketData);
		}
	}

	private async Task LoadRealTimeMarketData(IMarketDataService marketDataService)
	{
		var marketDataList = new List<MarketData>();
		foreach (var currencyPair in _currencyPairs)
		{
			var priceData = currencyPair is { From: CurrencyCode.USD, To: CurrencyCode.RUB }
				? await _centralBankRussiaApiClient.GetDailyMarketDataAsync()
				: await _binanceApiClient.GetMarketDataAsync(currencyPair.From, currencyPair.To, limit: 1);

			foreach (var data in priceData)
			{
				var marketData = new MarketData
				{
					From = currencyPair.From,
					To = currencyPair.To,
					Price = data.Price,
					Date = data.Date
				};

				marketDataList.Add(marketData);
			}
		}

		await marketDataService.SaveMarketDataAsync(marketDataList);
	}
}