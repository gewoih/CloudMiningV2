using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.DTO.Payments.Deposits;
using CloudMining.Interfaces.DTO.Statistics;
using CloudMining.Interfaces.DTO.Users;
using CloudMining.Interfaces.Interfaces;

namespace CloudMining.Application.Services;

public class ReceiveAndSellCalculationStrategy : IStatisticsCalculationStrategy
{
	private readonly IMarketDataService _marketDataService;
	private readonly IStatisticsHelper _statisticsHelper;

	public ReceiveAndSellCalculationStrategy(
		IMarketDataService marketDataService,
		IStatisticsHelper statisticsHelper)
	{
		_marketDataService = marketDataService;
		_statisticsHelper = statisticsHelper;
	}

	public async Task<List<StatisticsDto>> GetStatisticsAsync(List<ShareablePayment> payoutsList,
		List<ShareablePayment> electricityExpenseList,
		IEnumerable<CurrencyPair> uniqueCurrencyPairs,
		List<UserDto> userDtosList,
		Dictionary<Guid,List<DepositDto>>? usersDeposits)
	{
		var payoutsDates = GetPayoutsDates(payoutsList);
		var usdToRubRatesByDate = await _marketDataService.GetUsdToRubRatesByDateAsync(payoutsDates);
		var incomesPerUser = await GetPriceBarsAsync(payoutsList, usdToRubRatesByDate, payoutsDates,
			uniqueCurrencyPairs, userDtosList);
		var statisticsDtoList = await _statisticsHelper.GetStatisticsDtoList(incomesPerUser, electricityExpenseList, usersDeposits);

		return statisticsDtoList;
	}


	private async Task<Dictionary<UserDto, List<MonthlyPriceBar>>> GetPriceBarsAsync(
		List<ShareablePayment> payouts,
		Dictionary<DateOnly, decimal> usdToRubRate,
		List<DateTime> payoutsDates,
		IEnumerable<CurrencyPair> uniqueCurrencyPairs,
		List<UserDto> userDtosList)
	{
		var currencyRates =
			await _marketDataService.GetMarketDataForCurrenciesByDateAsync(uniqueCurrencyPairs, payoutsDates);

		var priceBarsPerUser = new Dictionary<UserDto, List<MonthlyPriceBar>>();

		foreach (var user in userDtosList)
		{
			var usdIncomeByDate = CalculateIncome(user.Id, payouts, currencyRates, CurrencyCode.USDT);
			var rubIncomeByDate = CalculateCurrencyIncomeByDate(usdIncomeByDate, usdToRubRate);
			var monthlyIncomeByDate = GroupIncomeByMonths(rubIncomeByDate);

			var userPriceBars = new List<MonthlyPriceBar>();

			foreach (var (date, incomeValue) in monthlyIncomeByDate)
			{
				userPriceBars.Add(new MonthlyPriceBar(incomeValue, date));
			}

			if (userPriceBars.Count != 0)
			{
				priceBarsPerUser[user] = userPriceBars;
			}
		}

		return priceBarsPerUser;
	}

	private static Dictionary<DateOnly, decimal> CalculateIncome(Guid userId, List<ShareablePayment> payouts,
		Dictionary<CurrencyPair, List<MarketData?>> currencyRates, CurrencyCode currency)
	{
		var incomes = new Dictionary<DateOnly, decimal>();

		foreach (var payout in payouts)
		{
			var userPaymentShare = payout.PaymentShares.FirstOrDefault(share => share.UserId == userId);
			if (userPaymentShare == null)
				continue;

			var currencyPair = new CurrencyPair
			{
				From = payout.Currency.Code,
				To = currency
			};
			if (!currencyRates.TryGetValue(currencyPair, out var marketDataList))
				continue;

			var payoutDate = DateOnly.FromDateTime(payout.Date);
			var currencyRate = marketDataList
				.Where(marketData => DateOnly.FromDateTime(marketData!.Date) == payoutDate)
				.Select(marketData => marketData!.Price)
				.FirstOrDefault();
			if (currencyRate == 0)
				continue;

			var income = currencyRate * userPaymentShare.Amount;
			if (!incomes.TryAdd(payoutDate, income))
				incomes[payoutDate] += income;
		}

		return incomes;
	}

	private static Dictionary<DateOnly, decimal> CalculateCurrencyIncomeByDate(Dictionary<DateOnly, decimal> incomes,
		Dictionary<DateOnly, decimal> rates)
	{
		var rubIncome = new Dictionary<DateOnly, decimal>();

		foreach (var (date, incomeValue) in incomes)
		{
			if (!rates.TryGetValue(date, out var rate))
				continue;

			if (rubIncome.ContainsKey(date))
				rubIncome[date] += incomeValue * rate;
			else
				rubIncome[date] = incomeValue * rate;
		}

		return rubIncome;
	}

	private static Dictionary<DateOnly, decimal> GroupIncomeByMonths(Dictionary<DateOnly, decimal> incomes)
	{
		var monthlyIncome = new Dictionary<DateOnly, decimal>();

		foreach (var (date, value) in incomes)
		{
			var firstDayOfMonth = new DateOnly(date.Year, date.Month, 1);

			if (!monthlyIncome.TryAdd(firstDayOfMonth, value))
				monthlyIncome[firstDayOfMonth] += value;
		}

		return monthlyIncome;
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