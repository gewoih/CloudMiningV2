using CloudMining.Interfaces.DTO.Statistics;

namespace CloudMining.Interfaces.Interfaces;

public interface IStatisticsService
{
   Task<List<StatisticsDto>> GetStatisticsListAsync();
}