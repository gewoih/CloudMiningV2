using CloudMining.Interfaces.DTO.Currencies;

namespace CloudMining.Interfaces.Interfaces;

public interface IMarketDataLoaderStrategyFactory
{
	IMarketDataLoaderStrategy Create(CurrencyPair currencyPair);
}