using CloudMining.Infrastructure.Settings;

namespace CloudMining.Interfaces.Interfaces;

public interface IMarketDataLoaderStrategyFactory
{
	IMarketDataLoaderStrategy Create(CurrencyPair currencyPair);
}