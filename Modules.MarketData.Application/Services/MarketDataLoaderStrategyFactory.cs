using CloudMining.Common.Settings;
using Microsoft.Extensions.DependencyInjection;
using Modules.Currencies.Domain.Enums;
using Modules.MarketData.Contracts.Interfaces;
using Modules.MarketData.Infrastructure.Settings;

namespace Modules.MarketData.Application.Services;

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