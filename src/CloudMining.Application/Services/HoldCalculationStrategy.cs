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
	private readonly IStatisticsHelper _statisticsHelper;
	private readonly IShareablePaymentService _shareablePaymentService;
	private readonly ICurrentUserService _currentUserService;

	public HoldCalculationStrategy(IShareablePaymentService shareablePaymentService,
		IMarketDataService marketDataService,
		IStatisticsHelper statisticsHelper,
		ICurrentUserService currentUserService)
	{
		_shareablePaymentService = shareablePaymentService;
		_marketDataService = marketDataService;
		_statisticsHelper = statisticsHelper;
		_currentUserService = currentUserService;
	}

	public async Task<List<StatisticsDto>> GetStatisticsAsync()
	{
		var isCurrentUserAdmin = _currentUserService.IsCurrentUserAdmin();
		
		var monthsSinceProjectStartDate = _statisticsHelper.CalculateMonthsSinceProjectStart();
		var usdToRubRate = await _marketDataService.GetLastUsdToRubRateAsync();
		var payoutsList = await _shareablePaymentService.GetAsync(paymentTypes: [ PaymentType.Crypto ], includePaymentShares: false);
		var uniqueCurrencyPairs = _statisticsHelper.GetUniqueCurrencyPairs(payoutsList);
		var incomes = await GetPriceBarsAsync(payoutsList, usdToRubRate, uniqueCurrencyPairs);
		var totalIncome = incomes.Sum(priceBar => priceBar.Value);
		var monthlyIncome = totalIncome / monthsSinceProjectStartDate;
		var expenses = await _shareablePaymentService.GetAsync(paymentTypes: [ PaymentType.Electricity, PaymentType.Purchase ], includePaymentShares: false);
		var spentOnElectricity = expenses.Where(payment => payment.Type == PaymentType.Electricity)
			.Sum(payment => payment.Amount);
		var spentOnPurchases = expenses.Where(payment => payment.Type == PaymentType.Purchase)
			.Sum(payment => payment.Amount);
		var totalExpense = spentOnElectricity + spentOnPurchases;
		var totalProfit = totalIncome - totalExpense;
		var monthlyProfit = totalProfit / monthsSinceProjectStartDate;
		var paybackPercent = totalExpense != 0 ? totalProfit / totalExpense * 100 : 0;
		var expensesList = _statisticsHelper.GetExpenses(expenses);
		var profits = _statisticsHelper.GetProfitsList(incomes, expensesList);

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
	

	private async Task<List<MonthlyPriceBar>> GetPriceBarsAsync(
		List<ShareablePayment> payouts, 
		decimal usdToRubRate,
		IEnumerable<CurrencyPair> uniqueCurrencyPairs)
	{
		var currentDate = DateTime.UtcNow;
		var firstPayoutDate = payouts
			.Select(payout => payout.Date)
			.MinBy(date => date);

		var currencyRates = await _marketDataService.GetLatestMarketDataForCurrenciesAsync(uniqueCurrencyPairs);

		var priceBars = new List<MonthlyPriceBar>();
		for (var processingDate = firstPayoutDate;
		     processingDate <= currentDate;
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
}