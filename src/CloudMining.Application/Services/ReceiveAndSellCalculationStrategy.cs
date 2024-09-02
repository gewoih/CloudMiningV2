using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.DTO.Statistics;
using CloudMining.Interfaces.Interfaces;


namespace CloudMining.Application.Services;

public class ReceiveAndSellCalculationStrategy : IStatisticsCalculationStrategy
{
	private readonly IMarketDataService _marketDataService;
	private readonly IStatisticsCalculationHelperService _statisticsCalculationHelperService;
	private readonly int _monthsSinceProjectStartDate;

	private readonly IShareablePaymentService _shareablePaymentService;

	public ReceiveAndSellCalculationStrategy(IShareablePaymentService shareablePaymentService,
		IMonthsCalculationService monthsCalculationService,
		IMarketDataService marketDataService,
		IStatisticsCalculationHelperService statisticsCalculationHelperService)
	{
		_shareablePaymentService = shareablePaymentService;
		_marketDataService = marketDataService;
		_statisticsCalculationHelperService = statisticsCalculationHelperService;
		_monthsSinceProjectStartDate = monthsCalculationService.CalculateSinceProjectStart();
	}

	public async Task<StatisticsDto> GetStatisticsAsync()
	{
		var payoutsList =
			await _shareablePaymentService.GetAsync(paymentTypes: [PaymentType.Crypto], includePaymentShares: false);
		var payoutsDates = GetPayoutsDates(payoutsList);
		var usdToRubRatesByDate = await _marketDataService.GetUsdToRubRatesByDateAsync(payoutsDates);
		var uniqueCurrencyPairs = _statisticsCalculationHelperService.GetUniqueCurrencyPairs(payoutsList);
		var incomes = await GetPriceBarsAsync(payoutsList, usdToRubRatesByDate, payoutsDates, uniqueCurrencyPairs);
		var totalIncome = incomes.Sum(priceBar => priceBar.Value);
		var monthlyIncome = totalIncome / _monthsSinceProjectStartDate;
		var expenses =
			await _shareablePaymentService.GetAsync(paymentTypes: [PaymentType.Electricity, PaymentType.Purchase],
				includePaymentShares: false);
		var spentOnElectricity = expenses.Where(payment => payment.Type == PaymentType.Electricity)
			.Sum(payment => payment.Amount);
		var spentOnPurchases = expenses.Where(payment => payment.Type == PaymentType.Purchase)
			.Sum(payment => payment.Amount);
		var totalExpense = spentOnElectricity + spentOnPurchases;
		var totalProfit = totalIncome - totalExpense;
		var monthlyProfit = totalProfit / _monthsSinceProjectStartDate;
		var paybackPercent = totalExpense != 0 ? totalProfit / totalExpense * 100 : 0;
		var expensesList = _statisticsCalculationHelperService.GetExpenses(expenses);
		var profits = _statisticsCalculationHelperService.GetProfitsList(incomes, expensesList);

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
		Dictionary<DateTime, decimal> usdToRubRate,
		List<DateTime> payoutsDates,
		IEnumerable<CurrencyPair> uniqueCurrencyPairs)
	{
		var currencyRates =
			await _marketDataService.GetMarketDataForCurrenciesByDateAsync(uniqueCurrencyPairs, payoutsDates);

		var usdIncomeByDate = CalculateIncome(payouts, currencyRates, CurrencyCode.USDT);
		var rubIncomeByDate = CalculateCurrencyIncomeByDate(usdIncomeByDate, usdToRubRate);

		var priceBars = new List<MonthlyPriceBar>();

		var monthlyIncomeByDate = rubIncomeByDate
			.GroupBy(entry => new DateOnly(entry.Key.Year, entry.Key.Month, 1))
			.ToDictionary(
				group => group.Key,
				group => group.Sum(entry => entry.Value));

		foreach (var (date, incomeValue) in monthlyIncomeByDate)
		{
			priceBars.Add(new MonthlyPriceBar(incomeValue, date));
		}

		return priceBars;
	}

	private static Dictionary<DateTime, decimal> CalculateIncome(List<ShareablePayment> payouts,
		Dictionary<CurrencyPair, List<MarketData?>> currencyRates, CurrencyCode currency)
	{
		var incomes = new Dictionary<DateTime, decimal>();

		foreach (var payout in payouts)
		{
			foreach (var (currencyPair, marketDataList) in currencyRates)
			{
				if (payout.Currency.Code != currencyPair.From || currencyPair.To != currency)
					continue;

				//TODO: Что за хуйня?
				var matchingMarketData = marketDataList
					.Find(marketData => marketData != null && payout.Date.Date.AddHours(payout.Date.Hour) ==
						marketData.Date);

				if (matchingMarketData == null)
					continue;

				var income = payout.Amount * matchingMarketData.Price;
				incomes[payout.Date] = income;
			}
		}

		return incomes;
	}

	private static Dictionary<DateTime, decimal> CalculateCurrencyIncomeByDate(Dictionary<DateTime, decimal> incomes,
		Dictionary<DateTime, decimal> rates)
	{
		var rubIncome = new Dictionary<DateTime, decimal>();

		foreach (var (dateTime, incomeValue) in incomes)
		{
			var date = dateTime.Date;

			if (!rates.TryGetValue(date, out var rate))
				continue;

			if (rubIncome.ContainsKey(date))
				rubIncome[date] += incomeValue * rate;
			else
				rubIncome[date] = incomeValue * rate;
		}

		return rubIncome;
	}

	private static List<DateTime> GetPayoutsDates(IEnumerable<ShareablePayment> payouts)
	{
		var payoutsDates = payouts
			.Select(payment => payment.Date) //TODO: DateOnly
			.Distinct()
			.ToList();
		return payoutsDates;
	}
}