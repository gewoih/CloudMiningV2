using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.DTO.Statistics;
using CloudMining.Interfaces.DTO.Users;
using CloudMining.Interfaces.Interfaces;

namespace CloudMining.Application.Services;

public class HoldCalculationStrategy : IStatisticsCalculationStrategy
{
	private readonly IMarketDataService _marketDataService;
	private readonly IStatisticsHelper _statisticsHelper;
	private readonly IShareablePaymentService _shareablePaymentService;
	private readonly ICurrentUserService _currentUserService;
	private readonly IUserManagementService _userManagementService;

	public HoldCalculationStrategy(IShareablePaymentService shareablePaymentService,
		IMarketDataService marketDataService,
		IStatisticsHelper statisticsHelper,
		ICurrentUserService currentUserService,
		IUserManagementService userManagementService)
	{
		_shareablePaymentService = shareablePaymentService;
		_marketDataService = marketDataService;
		_statisticsHelper = statisticsHelper;
		_currentUserService = currentUserService;
		_userManagementService = userManagementService;
	}

	public async Task<List<StatisticsDto>> GetStatisticsAsync()
	{
		var statisticsDtoList = new List<StatisticsDto>();

		var isCurrentUserAdmin = _currentUserService.IsCurrentUserAdmin();

		var payoutsList =
			await _shareablePaymentService.GetAsync(paymentTypes: [PaymentType.Crypto], adminCheck: false);
		var expenseList =
			await _shareablePaymentService.GetAsync(paymentTypes: [PaymentType.Electricity, PaymentType.Purchase],
				adminCheck: false);

		var monthsSinceProjectStartDate = _statisticsHelper.CalculateMonthsSinceProjectStart();
		var usdToRubRate = await _marketDataService.GetLastUsdToRubRateAsync();
		var uniqueCurrencyPairs = _statisticsHelper.GetUniqueCurrencyPairs(payoutsList);
		var incomesPerUser = await GetPriceBarsAsync(payoutsList, usdToRubRate, uniqueCurrencyPairs);

		foreach (var (user, priceBars) in incomesPerUser)
		{
			var totalIncome = priceBars.Sum(priceBar => priceBar.Value);
			var monthlyIncome = totalIncome / monthsSinceProjectStartDate;
			var spentOnElectricity = expenseList
				.Where(payment => payment.Type == PaymentType.Electricity &&
				                  payment.PaymentShares.Exists(share => share.UserId == user.Id))
				.Sum(payment => payment.PaymentShares
					.Where(share => share.UserId == user.Id)
					.Sum(share => share.Amount));

			var spentOnPurchases = expenseList
				.Where(payment => payment.Type == PaymentType.Purchase &&
				                  payment.PaymentShares.Exists(share => share.UserId == user.Id))
				.Sum(payment => payment.PaymentShares
					.Where(share => share.UserId == user.Id)
					.Sum(share => share.Amount));

			var totalExpense = spentOnElectricity + spentOnPurchases;
			var totalProfit = totalIncome - totalExpense;
			var monthlyProfit = totalProfit / monthsSinceProjectStartDate;
			var paybackPercent = totalExpense != 0 ? totalProfit / totalExpense * 100 : 0;
			var expensesList = _statisticsHelper.GetExpenses(expenseList, user.Id);
			var profits = _statisticsHelper.GetProfitsList(priceBars, expensesList);

			var statisticsDto = new UserStatisticsDto(
				user,
				totalIncome,
				monthlyIncome,
				totalExpense,
				spentOnElectricity,
				spentOnPurchases,
				totalProfit,
				monthlyProfit,
				paybackPercent,
				priceBars,
				profits,
				expensesList);

			statisticsDtoList.Add(statisticsDto);
		}

		if (!isCurrentUserAdmin) return statisticsDtoList;

		var generalStatisticsDto = _statisticsHelper.GetGeneralStatisticsDto(statisticsDtoList);
		statisticsDtoList.Insert(0, generalStatisticsDto);

		return statisticsDtoList;
	}


	private async Task<Dictionary<UserDto, List<MonthlyPriceBar>>> GetPriceBarsAsync(
		List<ShareablePayment> payouts,
		decimal usdToRubRate,
		IEnumerable<CurrencyPair> uniqueCurrencyPairs)
	{
		var userDtosList = await _userManagementService.GetUserDtosAsync();
		var isCurrentUserAdmin = _currentUserService.IsCurrentUserAdmin();
		if (!isCurrentUserAdmin)
		{
			var currentUserId = _currentUserService.GetCurrentUserId();
			userDtosList = userDtosList.Where(user => user.Id == currentUserId).ToList();
		}

		var currentDate = DateTime.UtcNow;
		var firstPayoutDate = payouts
			.Select(payout => payout.Date)
			.MinBy(date => date);

		var currencyRates = await _marketDataService.GetLatestMarketDataForCurrenciesAsync(uniqueCurrencyPairs);

		var priceBarsPerUser = new Dictionary<UserDto, List<MonthlyPriceBar>>();
		foreach (var user in userDtosList)
		{
			var userPriceBars = new List<MonthlyPriceBar>();

			for (var processingDate = firstPayoutDate;
			     processingDate <= currentDate;
			     processingDate = processingDate.AddMonths(1))
			{
				var incomeByCurrencies = GetIncomeByDate(user.Id, payouts, processingDate.Year, processingDate.Month);
				var usdIncome = CalculateTotalIncome(incomeByCurrencies, currencyRates);

				var rubIncome = usdIncome * usdToRubRate;
				if (rubIncome != 0)
					userPriceBars.Add(
						new MonthlyPriceBar(rubIncome, new DateOnly(processingDate.Year, processingDate.Month, 1)));
			}

			if (userPriceBars.Count != 0)
			{
				priceBarsPerUser[user] = userPriceBars;
			}
		}

		return priceBarsPerUser;
	}

	private static Dictionary<CurrencyCode, decimal> GetIncomeByDate(Guid userId, IEnumerable<ShareablePayment> payouts,
		int year, int month)
	{
		var payoutsForDate = payouts
			.Where(payout => payout.Date.Year == year && payout.Date.Month == month)
			.SelectMany(payout => payout.PaymentShares
				.Where(payoutShare => payoutShare.UserId == userId)
				.Select(payoutShare => new
				{
					payout.Currency.Code,
					payoutShare.Amount
				}))
			.GroupBy(share => share.Code);

		var incomeByCurrency = new Dictionary<CurrencyCode, decimal>();
		foreach (var sharedPayout in payoutsForDate)
		{
			var currencyCode = sharedPayout.Key;
			var totalAmount = sharedPayout.Sum(share => share.Amount);
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