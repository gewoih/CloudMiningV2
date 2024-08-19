using CloudMining.Domain.Enums;
using CloudMining.Interfaces.DTO.Statistics;
using CloudMining.Interfaces.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CloudMining.Application.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public StatisticsService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<StatisticsDto> GetStatisticsAsync(StatisticsCalculationStrategy strategy)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var statisticsCalculationStrategyFactory =
            scope.ServiceProvider.GetRequiredService<IStatisticsCalculationStrategyFactory>();
        var statisticsCalculationStrategy = statisticsCalculationStrategyFactory.Create(strategy);
        var statisticsDto = await statisticsCalculationStrategy.GetStatisticsAsync();
        return statisticsDto;
    }
}