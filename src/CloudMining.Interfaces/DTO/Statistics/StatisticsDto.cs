namespace CloudMining.Interfaces.DTO.Statistics;

public record StatisticsDto(
    decimal TotalIncome,
    decimal MonthlyIncome,
    decimal TotalExpense,
    decimal ElectricityExpense,
    decimal PurchaseExpense,
    decimal TotalProfit,
    decimal MonthlyProfit,
    decimal PaybackPercent,
    List<MonthlyPriceBar> Incomes,
    List<MonthlyPriceBar> Profits,
    List<Expense> Expenses
);