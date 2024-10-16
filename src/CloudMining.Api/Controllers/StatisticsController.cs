using CloudMining.Domain.Enums;
using CloudMining.Interfaces.DTO.Purchases;
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
    private readonly IPurchaseService _purchaseService;

    public StatisticsController(IStatisticsService statisticsService, IPurchaseService purchaseService)
    {
        _statisticsService = statisticsService;
        _purchaseService = purchaseService;
    }
    
    [HttpGet]
    public async Task<List<StatisticsDto>> Get([FromQuery] StatisticsCalculationStrategy statisticsCalculationStrategy)
    {
        var statisticsDtoList = await _statisticsService.GetStatisticsAsync(statisticsCalculationStrategy);
        return statisticsDtoList;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("purchases")]
    public async Task<IActionResult> CreatePurchase([FromBody] CreatePurchaseDto purchaseDto)
    {
        var succeeded = await _purchaseService.CreatePurchaseAsync(purchaseDto);
        if (!succeeded)
            return StatusCode(500, "An error occurred while creating the purchase.");

        return Ok();
    }
}