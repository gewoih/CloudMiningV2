using CloudMining.Interfaces.DTO.Users;

namespace CloudMining.Interfaces.DTO.Statistics;

public record UserStatisticsDto(
	UserDto User,
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
	List<Expense> Expenses)
	: StatisticsDto(
		TotalIncome,
		MonthlyIncome,
		TotalExpense,
		ElectricityExpense,
		PurchaseExpense,
		TotalProfit,
		MonthlyProfit,
		PaybackPercent,
		Incomes,
		Profits,
		Expenses);