using CloudMining.Interfaces.DTO.Users;

namespace CloudMining.Interfaces.DTO.Statistics;

public record UserStatisticsDto(
	UserDto User,
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
	List<Expense> Expenses)
	: StatisticsDto(
		TotalIncome,
		MonthlyIncome,
		TotalExpense,
		ElectricityExpense,
		DepositAmount,
		TotalProfit,
		MonthlyProfit,
		PaybackPercent,
		Incomes,
		Profits,
		Expenses);