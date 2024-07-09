using CloudMining.Domain.Enums;

namespace CloudMining.Interfaces.DTO.Statistics;

public record ChartDto(
    List<ChartDatasetDto> Datasets,
    List<string> TimeLabels,
    TimeLine TimeLine,
    ExpenseType? ExpenseType
);