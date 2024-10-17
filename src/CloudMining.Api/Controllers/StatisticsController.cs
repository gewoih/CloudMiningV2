using CloudMining.Application.Mappings;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Purchases;
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
    private readonly IMapper<Purchase, PurchaseDto> _purchaseMapper;

    public StatisticsController(IStatisticsService statisticsService,
        IPurchaseService purchaseService,
        IMapper<Purchase, PurchaseDto> purchaseMapper)
    {
        _statisticsService = statisticsService;
        _purchaseService = purchaseService;
        _purchaseMapper = purchaseMapper;
    }
    
    [HttpGet]
    public async Task<StatisticsPageDto> Get([FromQuery] StatisticsCalculationStrategy statisticsCalculationStrategy)
    {
        var statisticsDtoList = await _statisticsService.GetStatisticsAsync(statisticsCalculationStrategy);
        var purchaseDtoList = await _purchaseService.GetPurchasesAsync();
        
        var statisticsPage = new StatisticsPageDto(statisticsDtoList, purchaseDtoList);
        
        return statisticsPage;
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