using CloudMining.Interfaces.DTO.Purchases;

namespace CloudMining.Interfaces.DTO.Statistics;

public record StatisticsPageDto(List<StatisticsDto> StatisticsDtoList, List<PurchaseDto> PurchaseDtoList);