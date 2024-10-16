using CloudMining.Application.Mappings;
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
    private readonly PurchaseMapper _purchaseMapper;

    public StatisticsController(IStatisticsService statisticsService,
        IPurchaseService purchaseService,
        PurchaseMapper purchaseMapper)
    {
        _statisticsService = statisticsService;
        _purchaseService = purchaseService;
        _purchaseMapper = purchaseMapper;
    }
    
    [HttpGet]
    public async Task<List<StatisticsDto>> Get([FromQuery] StatisticsCalculationStrategy statisticsCalculationStrategy)
    {
        var statisticsDtoList = await _statisticsService.GetStatisticsAsync(statisticsCalculationStrategy);
        return statisticsDtoList;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("purchases")]
    public async Task<PurchaseDto> CreatePurchase([FromBody] CreatePurchaseDto createPurchaseDto)
    {
        var newPurchase = await _purchaseService.CreatePurchaseAsync(createPurchaseDto);
        var purchaseDto = _purchaseMapper.ToDto(newPurchase);
        
        return purchaseDto;
    }
}