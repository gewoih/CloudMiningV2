namespace CloudMining.Interfaces.DTO.Statistics;

public record ChartDatasetDto(
    string Title,
    List<decimal> Data
);