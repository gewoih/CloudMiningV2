using CloudMining.Domain.Enums;
using CloudMining.Interfaces.DTO.Statistics;

namespace CloudMining.Interfaces.Interfaces;

public interface IStatisticsService
{
   Task<List<StatisticsDto>> GetStatisticsAsync(StatisticsCalculationStrategy strategy);
}