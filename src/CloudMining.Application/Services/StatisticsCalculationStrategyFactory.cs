using CloudMining.Domain.Enums;
using CloudMining.Interfaces.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CloudMining.Application.Services;

public sealed class StatisticsCalculationStrategyFactory : IStatisticsCalculationStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;

    public StatisticsCalculationStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IStatisticsCalculationStrategy Create(StatisticsCalculationStrategy strategy)
    {
        return strategy is StatisticsCalculationStrategy.Hold
            ? _serviceProvider.GetRequiredService<HoldCalculationStrategy>()
            : _serviceProvider.GetRequiredService<ReceiveAndSellCalculationStrategy>();
    }
}