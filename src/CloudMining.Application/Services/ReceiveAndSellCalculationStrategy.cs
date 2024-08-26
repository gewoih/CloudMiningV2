using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Infrastructure.Settings;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.DTO.Statistics;
using CloudMining.Interfaces.Interfaces;
using Microsoft.Extensions.Options;

namespace CloudMining.Application.Services;

public class ReceiveAndSellCalculationStrategy : IStatisticsCalculationStrategy
{
	private readonly DateTime _currentDate = DateTime.UtcNow;
	private readonly IMarketDataService _marketDataService;
	private readonly int _monthsSinceProjectStartDate;
	private readonly DateTime _projectStartDate;
	private readonly IShareablePaymentService _shareablePaymentService;


	public ReceiveAndSellCalculationStrategy(IShareablePaymentService shareablePaymentService,
		IOptions<ProjectInformationSettings> projectInformation,
		IMarketDataService marketDataService)
	{
		_shareablePaymentService = shareablePaymentService;
		_marketDataService = marketDataService;
		_projectStartDate = DateTime.SpecifyKind(projectInformation.Value.ProjectStartDate, DateTimeKind.Utc);
		_monthsSinceProjectStartDate = CalculateMonthsSinceProjectStart();
	}

	public async Task<StatisticsDto> GetStatisticsAsync()
	{
		var payoutsList =
			await _shareablePaymentService.GetAsync(paymentTypes: [PaymentType.Crypto], includePaymentShares: false);
		var payoutsDates = GetPayoutsDates(payoutsList);
		var usdToRubRatesByDate = await _marketDataService.GetUsdToRubRatesByDateAsync(payoutsDates);
		var incomes = await GetPriceBarsAsync(payoutsList, usdToRubRatesByDate, payoutsDates);
		var totalIncome = incomes.Sum(priceBar => priceBar.Value);
		var monthlyIncome = totalIncome / _monthsSinceProjectStartDate;
		var expenses = await _shareablePaymentService.GetAsync(
			paymentTypes: [PaymentType.Electricity, PaymentType.Purchase],
			includePaymentShares: false);
		var spentOnElectricity = expenses.Where(payment => payment.Type == PaymentType.Electricity)
			.Sum(payment => payment.Amount);
		var spentOnPurchases = expenses.Where(payment => payment.Type == PaymentType.Purchase)
			.Sum(payment => payment.Amount);
		var totalExpense = spentOnElectricity + spentOnPurchases;
		var totalProfit = totalIncome - totalExpense;
		var monthlyProfit = totalProfit / _monthsSinceProjectStartDate;
		var paybackPercent = totalExpense != 0 ? totalProfit / totalExpense * 100 : 0;
		var expensesList = GetExpenses(expenses);
		var profits = GetProfitsList(incomes, expensesList);

		var statisticsDto = new StatisticsDto(
			totalIncome,
			monthlyIncome,
			totalExpense,
			spentOnElectricity,
			spentOnPurchases,
			totalProfit,
			monthlyProfit,
			paybackPercent,
			incomes,
			profits,
			expensesList);

		return statisticsDto;
	}

	private int CalculateMonthsSinceProjectStart()
	{
		var totalMonths = (_currentDate.Year - _projectStartDate.Year) * 12 + _currentDate.Month -
		                  _projectStartDate.Month;
		if (_currentDate.Day < _projectStartDate.Day)
			totalMonths--;

		return totalMonths;
	}

	private async Task<List<MonthlyPriceBar>> GetPriceBarsAsync(
		List<ShareablePayment> payouts,
		Dictionary<DateTime, decimal> usdToRubRate,
		List<DateTime> payoutsDates)
	{
		var uniqueCurrencyPairs = payouts
			.Select(payment => new CurrencyPair { From = payment.Currency.Code, To = CurrencyCode.USDT })
			.Distinct()
			.ToList();

		var currencyRates =
			await _marketDataService.GetMarketDataForCurrenciesByDateAsync(uniqueCurrencyPairs, payoutsDates);

		var usdIncomeByDate = GetUsdIncomeByDate(payouts, currencyRates);
		var rubIncomeByDate = GetRubIncomeByDate(usdIncomeByDate, usdToRubRate);

		var priceBars = new List<MonthlyPriceBar>();

		var monthlyIncomeByDate = rubIncomeByDate
			.GroupBy(entry => new DateOnly(entry.Key.Year, entry.Key.Month, 1))
			.ToDictionary(
				group => group.Key,
				group => group.Sum(entry => entry.Value));

		foreach (var (date, incomeValue) in monthlyIncomeByDate)
		{
			priceBars.Add(
				new MonthlyPriceBar(incomeValue, date));
		}

		return priceBars;
	}


	private static Dictionary<DateTime, decimal> GetUsdIncomeByDate(
		List<ShareablePayment> payouts,
		Dictionary<CurrencyPair, List<MarketData?>> currencyRates,
		CurrencyCode toCurrency = CurrencyCode.USDT)
	{
		var usdIncomeByDate = new Dictionary<DateTime, decimal>();

		foreach (var payout in payouts)
		{
			foreach (var (currencyPair, marketDataList) in currencyRates)
			{
				if (payout.Currency.Code != currencyPair.From || currencyPair.To != toCurrency) continue;

				var matchingMarketData = marketDataList
					.Find(marketData =>
						marketData != null &&
						payout.Date.Date.AddHours(payout.Date.Hour) == marketData.Date);

				if (matchingMarketData == null) continue;

				var incomeInUsd = payout.Amount * matchingMarketData.Price;

				usdIncomeByDate[payout.Date] = incomeInUsd;
			}
		}

		return usdIncomeByDate;
	}

	private static Dictionary<DateTime, decimal> GetRubIncomeByDate(Dictionary<DateTime, decimal> usdIncome,
		IReadOnlyDictionary<DateTime, decimal> usdToRubRate)
	{
		var rubIncome = new Dictionary<DateTime, decimal>();

		foreach (var (dateTime, incomeValue) in usdIncome)
		{
			var date = dateTime.Date;

			if (!usdToRubRate.TryGetValue(date, out var rate)) continue;

			if (rubIncome.ContainsKey(date))
			{
				rubIncome[date] += incomeValue * rate;
			}
			else
			{
				rubIncome[date] = incomeValue * rate;
			}
		}

		return rubIncome;
	}


	private List<Expense> GetExpenses(List<ShareablePayment> payments)
	{
		var expenseList = new List<Expense>();
		var electricityPayments = payments.Where(payment => payment.Type == PaymentType.Electricity);
		var purchases = payments.Where(payment => payment.Type == PaymentType.Purchase);

		var electricityMonthlyPriceBars = CalculateMonthlyExpenses(electricityPayments);
		var purchasesMonthlyPriceBars = CalculateMonthlyExpenses(purchases);

		var totalMonthlyPriceBars = electricityMonthlyPriceBars.Concat(purchasesMonthlyPriceBars);
		var generalExpensePriceBars = CalculateGeneralMonthlyExpenses(totalMonthlyPriceBars);

		expenseList.Add(new Expense(ExpenseType.OnlyElectricity, electricityMonthlyPriceBars));
		expenseList.Add(new Expense(ExpenseType.OnlyPurchases, purchasesMonthlyPriceBars));
		expenseList.Add(new Expense(ExpenseType.Total, generalExpensePriceBars));

		return expenseList;
	}

	private List<MonthlyPriceBar> CalculateMonthlyExpenses(IEnumerable<ShareablePayment> payments)
	{
		var monthlyExpenses = payments
			.GroupBy(payment => (payment.Date.Year, payment.Date.Month))
			.ToList();

		var priceBars = new List<MonthlyPriceBar>();
		var processingDate = _projectStartDate;

		while (processingDate <= _currentDate)
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

	private static List<MonthlyPriceBar> GetProfitsList(List<MonthlyPriceBar> incomes, IEnumerable<Expense> expenses)
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

	private static List<DateTime> GetPayoutsDates(IEnumerable<ShareablePayment> payouts)
	{
		var payoutsDates = payouts
			.Select(payment => payment.Date)
			.Distinct()
			.ToList();
		return payoutsDates;
	}
}