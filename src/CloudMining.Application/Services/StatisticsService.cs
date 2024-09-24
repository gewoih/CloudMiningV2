using CloudMining.Domain.Enums;
using CloudMining.Interfaces.DTO.Statistics;
using CloudMining.Interfaces.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CloudMining.Application.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IStatisticsCalculationStrategyFactory _statisticsCalculationStrategyFactory;

    public StatisticsService(IStatisticsCalculationStrategyFactory statisticsCalculationStrategyFactory)
    {
        _statisticsCalculationStrategyFactory = statisticsCalculationStrategyFactory;
    }

    public async Task<List<StatisticsDto>> GetStatisticsAsync(StatisticsCalculationStrategy strategy)
    {
        var statisticsCalculationStrategy = _statisticsCalculationStrategyFactory.Create(strategy);
        var statisticsDto = await statisticsCalculationStrategy.GetStatisticsAsync();
        return statisticsDto;
    }
}