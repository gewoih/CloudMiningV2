using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.DTO.Statistics;
using CloudMining.Interfaces.Interfaces;

namespace CloudMining.Application.Services;

public class HoldCalculationStrategy : IStatisticsCalculationStrategy
{
	private readonly IMarketDataService _marketDataService;
	private readonly int _monthsSinceProjectStartDate;
	private readonly IShareablePaymentService _shareablePaymentService;

	public HoldCalculationStrategy(IShareablePaymentService shareablePaymentService,
		IMarketDataService marketDataService)
	{
		_shareablePaymentService = shareablePaymentService;
		_marketDataService = marketDataService;
		_monthsSinceProjectStartDate = CalculateMonthsSinceProjectStart();
	}

	public async Task<StatisticsDto> GetStatisticsAsync()
	{
		var usdToRubRate = await _marketDataService.GetLastUsdToRubRateAsync();
		var payoutsList = await _shareablePaymentService.GetAsync(paymentTypes: [ PaymentType.Crypto ], includePaymentShares: false);
		var incomes = await GetPriceBarsAsync(payoutsList, usdToRubRate);
		var totalIncome = incomes.Sum(priceBar => priceBar.Value);
		var monthlyIncome = totalIncome / _monthsSinceProjectStartDate;
		var expenses = await _shareablePaymentService.GetAsync(paymentTypes: [ PaymentType.Electricity, PaymentType.Purchase ], includePaymentShares: false);
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
		decimal usdToRubRate)
	{
		var uniqueCurrencyPairs = payouts
			.Select(payment => new CurrencyPair { From = payment.Currency.Code, To = CurrencyCode.USDT })
			.Distinct()
			.ToList();

		var currencyRates = await _marketDataService.GetLatestMarketDataForCurrenciesAsync(uniqueCurrencyPairs);

		var priceBars = new List<MonthlyPriceBar>();
		for (var processingDate = _projectStartDate;
		     processingDate <= _currentDate;
		     processingDate = processingDate.AddMonths(1))
		{
			var incomeByCurrencies = GetIncomeByDate(payouts, processingDate.Year, processingDate.Month);
			var usdIncome = CalculateTotalIncome(incomeByCurrencies, currencyRates);

			var rubIncome = usdIncome * usdToRubRate;
			if (rubIncome != 0)
				priceBars.Add(
					new MonthlyPriceBar(rubIncome, new DateOnly(processingDate.Year, processingDate.Month, 1)));
		}

		return priceBars;
	}

	private static Dictionary<CurrencyCode, decimal> GetIncomeByDate(IEnumerable<ShareablePayment> payouts, int year,
		int month)
	{
		var payoutsForDate = payouts
			.Where(payment => payment.Date.Year == year && payment.Date.Month == month)
			.GroupBy(payment => payment.Currency.Code);

		var incomeByCurrency = new Dictionary<CurrencyCode, decimal>();
		foreach (var group in payoutsForDate)
		{
			var currencyCode = group.Key;
			var totalAmount = group.Sum(payment => payment.Amount);
			incomeByCurrency[currencyCode] = totalAmount;
		}

		return incomeByCurrency;
	}

	private static decimal CalculateTotalIncome(IReadOnlyDictionary<CurrencyCode, decimal> incomeByCurrencies,
		Dictionary<CurrencyPair, MarketData?> currencyRates, CurrencyCode toCurrency = CurrencyCode.USDT)
	{
		var totalIncome = 0m;
		foreach (var (currencyCode, incomeAmount) in incomeByCurrencies)
		{
			var requiredCurrencyPair = new CurrencyPair { From = currencyCode, To = toCurrency };

			if (currencyRates.TryGetValue(requiredCurrencyPair, out var marketData) && marketData != null)
				totalIncome += incomeAmount * marketData.Price;
		}

		return totalIncome;
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
}