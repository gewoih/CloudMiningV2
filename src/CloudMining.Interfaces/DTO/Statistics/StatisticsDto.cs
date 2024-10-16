using CloudMining.Interfaces.DTO.Purchases;

namespace CloudMining.Interfaces.DTO.Statistics;

public record StatisticsDto(
    decimal TotalIncome,
    decimal MonthlyIncome,
    decimal TotalExpense,
    decimal ElectricityExpense,
    decimal DepositAmount,
    decimal TotalProfit,
    decimal MonthlyProfit,
    decimal PaybackPercent,
    List<MonthlyPriceBar> Incomes,
    List<MonthlyPriceBar> Profits,
    List<Expense> Expenses,
    List<PurchaseDto>? Purchases
    );