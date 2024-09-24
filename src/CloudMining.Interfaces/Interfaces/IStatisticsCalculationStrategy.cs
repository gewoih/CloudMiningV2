using CloudMining.Domain.Enums;
using CloudMining.Interfaces.DTO.Statistics;

namespace CloudMining.Interfaces.Interfaces;

public interface IStatisticsCalculationStrategy
{
    Task<List<StatisticsDto>> GetStatisticsAsync();
}