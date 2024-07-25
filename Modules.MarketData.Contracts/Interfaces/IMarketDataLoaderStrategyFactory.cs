using CloudMining.Common.Settings;
using Modules.MarketData.Infrastructure.Settings;

namespace Modules.MarketData.Contracts.Interfaces;

public interface IMarketDataLoaderStrategyFactory
{
	IMarketDataLoaderStrategy Create(CurrencyPair currencyPair);
}