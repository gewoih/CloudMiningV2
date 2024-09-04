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

	public List<Expense> GetExpenses(List<ShareablePayment> payments)
	{
		var expenseList = new List<Expense>();
		var electricityPayments = payments.Where(payment => payment.Type == PaymentType.Electricity).ToList();
		var purchases = payments.Where(payment => payment.Type == PaymentType.Purchase).ToList();

		var electricityMonthlyPriceBars = CalculateMonthlyExpenses(electricityPayments);
		var purchasesMonthlyPriceBars = CalculateMonthlyExpenses(purchases);

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

	private static List<MonthlyPriceBar> CalculateMonthlyExpenses(List<ShareablePayment> payments)
	{
		var priceBars = new List<MonthlyPriceBar>();
		if (payments.Count == 0)
			return priceBars;

		var monthlyExpenses = payments
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
}