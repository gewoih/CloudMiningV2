using CloudMining.Domain.Enums;
using CloudMining.Interfaces.DTO.Statistics;
using CloudMining.Interfaces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }
    
    [HttpGet]
    public async Task<List<StatisticsDto>> Get([FromQuery] StatisticsCalculationStrategy statisticsCalculationStrategy)
    {
        var statisticsDtoList = await _statisticsService.GetStatisticsAsync(statisticsCalculationStrategy);
        return statisticsDtoList;
    }
}