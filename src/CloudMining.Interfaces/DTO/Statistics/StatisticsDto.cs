using CloudMining.Domain.Enums;

namespace CloudMining.Interfaces.DTO.Statistics;

public record StatisticsDto(
    IncomeType IncomeType,
    decimal TotalIncome,
    decimal MonthlyIncome,
    decimal TotalExpense,
    decimal ElectricityExpense,
    decimal PurchaseExpense,
    decimal TotalProfit,
    decimal MonthlyProfit,
    decimal PaybackPercent,
    List<ChartDto> Charts
    );