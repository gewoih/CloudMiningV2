using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.DTO.Payments.Deposits;
using CloudMining.Interfaces.DTO.Statistics;
using CloudMining.Interfaces.DTO.Users;
using CloudMining.Interfaces.Interfaces;

namespace CloudMining.Application.Services;

public class HoldCalculationStrategy : IStatisticsCalculationStrategy
{
	private readonly IMarketDataService _marketDataService;
	private readonly IStatisticsHelper _statisticsHelper;

	public HoldCalculationStrategy(
		IMarketDataService marketDataService,
		IStatisticsHelper statisticsHelper)
	{
		_marketDataService = marketDataService;
		_statisticsHelper = statisticsHelper;
	}

	public async Task<List<StatisticsDto>> GetStatisticsAsync(
		List<ShareablePayment> payoutsList,
		List<ShareablePayment> electricityExpenseList,
		IEnumerable<CurrencyPair> uniqueCurrencyPairs,
		List<UserDto> userDtosList,
		Dictionary<Guid,List<DepositDto>>? usersDeposits)
	{
		var usdToRubRate = await _marketDataService.GetLastUsdToRubRateAsync();
		var incomesPerUser = await GetPriceBarsAsync(payoutsList, usdToRubRate, uniqueCurrencyPairs, userDtosList);
		var statisticsDtoList = _statisticsHelper.GetStatisticsDtoList(incomesPerUser, electricityExpenseList, usersDeposits);
		
		return statisticsDtoList;
	}


	private async Task<Dictionary<UserDto, List<MonthlyPriceBar>>> GetPriceBarsAsync(
		List<ShareablePayment> payouts,
		decimal usdToRubRate,
		IEnumerable<CurrencyPair> uniqueCurrencyPairs,
		List<UserDto> userDtosList)
	{
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