using CloudMining.Domain.Enums;

namespace CloudMining.Interfaces.DTO.Statistics;

public record ChartDatasetDto(
    ChartDataTitle Title,
    List<decimal> Data
);