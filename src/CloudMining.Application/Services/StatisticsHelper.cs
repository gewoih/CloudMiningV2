using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Infrastructure.Settings;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.DTO.Statistics;
using CloudMining.Interfaces.Interfaces;
using Microsoft.Extensions.Options;

namespace CloudMining.Application.Services;

public class StatisticsHelper : IStatisticsHelper
{
	private readonly DateOnly _projectStartDate;

	public StatisticsHelper(IOptions<ProjectInformationSettings> projectInformation)
	{
		_projectStartDate = projectInformation.Value.ProjectStartDate;
	}

	public List<CurrencyPair> GetUniqueCurrencyPairs(IEnumerable<ShareablePayment> payouts)
	{
		var uniqueCurrencyPairs = payouts
			.Select(payment => new CurrencyPair { From = payment.Currency.Code, To = CurrencyCode.USDT })
			.Distinct()
			.ToList();
		return uniqueCurrencyPairs;
	}

	public List<Expense> GetExpenses(List<ShareablePayment> payments, Guid? userId = null)
	{
		var expenseList = new List<Expense>();
		var electricityPayments = payments.Where(payment => payment.Type == PaymentType.Electricity).ToList();
		var purchases = payments.Where(payment => payment.Type == PaymentType.Purchase).ToList();

		if (userId != null)
		{
			electricityPayments = electricityPayments
				.Where(payment => payment.PaymentShares
					.Exists(share => share.UserId == userId))
				.ToList();
			purchases = purchases
				.Where(payment => payment.PaymentShares
					.Exists(share => share.UserId == userId))
				.ToList();
		}

		var electricityMonthlyPriceBars = CalculateMonthlyExpenses(electricityPayments, userId);
		var purchasesMonthlyPriceBars = CalculateMonthlyExpenses(purchases, userId);

		var totalMonthlyPriceBars = electricityMonthlyPriceBars.Concat(purchasesMonthlyPriceBars);
		var generalExpensePriceBars = CalculateGeneralMonthlyExpenses(totalMonthlyPriceBars);

		expenseList.Add(new Expense(ExpenseType.OnlyElectricity, electricityMonthlyPriceBars));
		expenseList.Add(new Expense(ExpenseType.OnlyPurchases, purchasesMonthlyPriceBars));
		expenseList.Add(new Expense(ExpenseType.Total, generalExpensePriceBars));

		return expenseList;
	}

	public List<MonthlyPriceBar> GetProfitsList(List<MonthlyPriceBar> incomes, IEnumerable<Expense> expenses)
	{
		var generalExpenses = expenses
			.Where(expense => expense.Type == ExpenseType.Total)
			.SelectMany(expense => expense.PriceBars)
			.ToList();

		if (generalExpenses.Count == 0)
			return incomes;

		var profits = incomes
			.Concat(generalExpenses.Select(expense => expense with { Value = -expense.Value }))
			.GroupBy(priceBar => priceBar.Date)
			.Select(group => new MonthlyPriceBar(
				group.Sum(priceBar => priceBar.Value),
				group.Key))
			.ToList();

		return profits;
	}

	private static List<MonthlyPriceBar> CalculateMonthlyExpenses(IReadOnlyCollection<ShareablePayment> payments, Guid? userId = null)
	{
		var priceBars = new List<MonthlyPriceBar>();
		if (payments.Count == 0)
			return priceBars;

		var monthlyExpenses = payments
			.SelectMany(payment =>
				userId == null
					? new[] { payment }
					: payment.PaymentShares
						.Where(share => share.UserId == userId)
						.Select(share => new ShareablePayment
						{
							Date = payment.Date,
							Amount = share.Amount
						})
			)
			.GroupBy(payment => (payment.Date.Year, payment.Date.Month))
			.ToList();

		var firstPaymentDate = payments
			.Select(payout => payout.Date)
			.MinBy(date => date);

		var currentDate = DateOnly.FromDateTime(DateTime.Now);
		var processingDate = DateOnly.FromDateTime(firstPaymentDate);

		while (processingDate <= currentDate)
		{
			var totalAmount = monthlyExpenses
				.Where(expense =>
					expense.Key.Year == processingDate.Year &&
					expense.Key.Month == processingDate.Month)
				.Sum(expense => expense.Sum(payment => payment.Amount));

			if (totalAmount != 0)
				priceBars.Add(new MonthlyPriceBar(totalAmount,
					new DateOnly(processingDate.Year, processingDate.Month, 1)));

			processingDate = processingDate.AddMonths(1);
		}

		return priceBars;
	}

	private static List<MonthlyPriceBar> CalculateGeneralMonthlyExpenses(IEnumerable<MonthlyPriceBar> monthlyPriceBars)
	{
		var generalExpensePriceBars = monthlyPriceBars
			.GroupBy(priceBar => priceBar.Date)
			.Select(group => new MonthlyPriceBar(
				group.Sum(priceBar => priceBar.Value),
				group.Key
			))
			.ToList();

		return generalExpensePriceBars;
	}

	public int CalculateMonthsSinceProjectStart()
	{
		var currentDate = DateTime.UtcNow;

		var totalMonths = (currentDate.Year - _projectStartDate.Year) * 12 + currentDate.Month -
		                  _projectStartDate.Month;
		if (currentDate.Day < _projectStartDate.Day)
			totalMonths--;

		return totalMonths;
	}

	public StatisticsDto GetGeneralStatisticsDto(List<StatisticsDto> statisticsDtoList)
	{
		var totalIncome = 0m;
		var monthlyIncome = 0m;
		var totalExpense = 0m;
		var electricityExpense = 0m;
		var purchaseExpense = 0m;
		var totalProfit = 0m;
		var monthlyProfit = 0m;
		var paybackPercent = 0m;

		var incomeSumsByDate = new Dictionary<DateOnly, decimal>();
		var profitSumsByDate = new Dictionary<DateOnly, decimal>();
		var expenseSumsByTypeAndDate = new Dictionary<(ExpenseType, DateOnly), decimal>();

		foreach (var statisticsDto in statisticsDtoList)
		{
			totalIncome += statisticsDto.TotalIncome;
			monthlyIncome += statisticsDto.MonthlyIncome;
			totalExpense += statisticsDto.TotalExpense;
			electricityExpense += statisticsDto.ElectricityExpense;
			purchaseExpense += statisticsDto.PurchaseExpense;
			totalProfit += statisticsDto.TotalProfit;
			monthlyProfit += statisticsDto.MonthlyProfit;
			paybackPercent += statisticsDto.PaybackPercent;

			foreach (var income in statisticsDto.Incomes)
			{
				incomeSumsByDate.TryAdd(income.Date, 0);
				incomeSumsByDate[income.Date] += income.Value;
			}

			foreach (var profit in statisticsDto.Profits)
			{
				profitSumsByDate.TryAdd(profit.Date, 0);
				profitSumsByDate[profit.Date] += profit.Value;
			}

			foreach (var expense in statisticsDto.Expenses)
			{
				foreach (var priceBar in expense.PriceBars)
				{
					var key = (expense.Type, priceBar.Date);
					expenseSumsByTypeAndDate.TryAdd(key, 0);
					expenseSumsByTypeAndDate[key] += priceBar.Value;
				}
			}
		}

		var totalIncomes = incomeSumsByDate
			.Select(x => new MonthlyPriceBar(x.Value, x.Key))
			.ToList();
		var totalProfits = profitSumsByDate
			.Select(x => new MonthlyPriceBar(x.Value, x.Key))
			.ToList();

		var totalExpenses = expenseSumsByTypeAndDate
			.GroupBy(x => x.Key.Item1)
			.Select(g => new Expense(g.Key, g
				.Select(x => new MonthlyPriceBar(x.Value, x.Key.Item2)).ToList()))
			.ToList();

		return new StatisticsDto(
			totalIncome,
			monthlyIncome,
			totalExpense,
			electricityExpense,
			purchaseExpense,
			totalProfit,
			monthlyProfit,
			paybackPercent,
			totalIncomes,
			totalProfits,
			totalExpenses
		);
	}
}