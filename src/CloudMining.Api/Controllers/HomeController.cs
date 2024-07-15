using CloudMining.Domain.Enums;
using CloudMining.Interfaces.DTO.Statistics;
using CloudMining.Interfaces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudMining.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class HomeController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public HomeController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }
    
    [HttpGet]
    public async Task<StatisticsDto> Get([FromQuery] IncomeType incomeType)
    {

        var statisticsDto = await _statisticsService.GetStatisticsAsync(incomeType);
        return statisticsDto;

    }

}