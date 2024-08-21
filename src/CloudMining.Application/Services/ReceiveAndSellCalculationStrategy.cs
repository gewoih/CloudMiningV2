using CloudMining.Interfaces.DTO.Statistics;
using CloudMining.Interfaces.Interfaces;

namespace CloudMining.Application.Services;

public class ReceiveAndSellCalculationStrategy : IStatisticsCalculationStrategy
{
    public Task<StatisticsDto> GetStatisticsAsync()
    {
        throw new NotImplementedException();
    }
}