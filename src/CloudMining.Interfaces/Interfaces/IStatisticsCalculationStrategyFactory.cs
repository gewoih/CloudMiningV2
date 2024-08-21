using CloudMining.Domain.Enums;

namespace CloudMining.Interfaces.Interfaces;

public interface IStatisticsCalculationStrategyFactory
{
    IStatisticsCalculationStrategy Create(StatisticsCalculationStrategy strategy);
}