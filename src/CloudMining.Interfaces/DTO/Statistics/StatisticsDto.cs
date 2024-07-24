using CloudMining.Domain.Enums;

namespace CloudMining.Interfaces.DTO.Statistics;

public record StatisticsDto(
    StatisticsCalculationStrategy StatisticsCalculationStrategy,
    decimal TotalIncome,
    decimal MonthlyIncome,
    decimal TotalExpense,
    decimal ElectricityExpense,
    decimal PurchaseExpense,
    decimal TotalProfit,
    decimal MonthlyProfit,
    decimal PaybackPercent,
    List<PriceBar> Incomes,
    List<PriceBar> Profits,
    List<Expense> Expenses
);