using CloudMining.Domain.Enums;
using CloudMining.Infrastructure.Settings;
using CloudMining.Interfaces.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CloudMining.Application.Services;

public sealed class MarketDataLoaderStrategyFactory : IMarketDataLoaderStrategyFactory
{
	private readonly IServiceProvider _serviceProvider;

	public MarketDataLoaderStrategyFactory(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}
	
	public IMarketDataLoaderStrategy Create(CurrencyPair currencyPair)
	{
		return currencyPair is { From: CurrencyCode.USD, To: CurrencyCode.RUB }
			? _serviceProvider.GetRequiredService<FiatMarketDataLoaderStrategy>()
			: _serviceProvider.GetRequiredService<CryptoMarketDataLoaderStrategy>();
	}
}