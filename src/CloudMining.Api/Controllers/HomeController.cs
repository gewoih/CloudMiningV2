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
    public async Task<HomePageDto> Get()
    {

        var statisticList = await _statisticsService.GetStatisticsListAsync();
        var homePageDto = new HomePageDto(statisticList);
        return homePageDto;

    }

}