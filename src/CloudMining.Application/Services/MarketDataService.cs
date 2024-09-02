using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Infrastructure.Database;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services;

public sealed class MarketDataService : IMarketDataService
{
	private readonly CloudMiningContext _context;

	public MarketDataService(CloudMiningContext context)
	{
		_context = context;
	}

	public async Task SaveMarketDataAsync(List<MarketData> marketData)
	{
		var existingCombinations = await _context.MarketData
			.GroupBy(data => new { data.From, data.To })
			.Select(group => new { group.Key.From, group.Key.To, MaxDate = group.Max(x => x.Date) })
			.ToListAsync();

		var existingCombinationsHashSet = existingCombinations
			.Select(data => (data.From, data.To, data.MaxDate))
			.ToHashSet();

		foreach (var data in marketData)
		{
			var combo = (data.From, data.To, data.Date);
			if (!existingCombinationsHashSet.Contains(combo))
				await _context.MarketData.AddAsync(data);
		}

		await _context.SaveChangesAsync()
			.ConfigureAwait(false);
	}

	public async Task<DateTime?> GetLastMarketDataDateAsync(CurrencyCode fromCurrency, CurrencyCode toCurrency)
	{
		var lastMarketDataDate = await _context.MarketData
			.Where(marketData => marketData.From == fromCurrency && marketData.To == toCurrency)
			.MaxAsync(marketData => (DateTime?)marketData.Date);

		return lastMarketDataDate;
	}

	public async Task<Dictionary<CurrencyPair, MarketData?>> GetLatestMarketDataForCurrenciesAsync(
		IEnumerable<CurrencyPair> currencyPairs)
	{
		var result = new Dictionary<CurrencyPair, MarketData?>();

		foreach (var currencyPair in currencyPairs)
		{
			var latestMarketData = await _context.MarketData
				.Where(marketData => marketData.From == currencyPair.From && marketData.To == currencyPair.To)
				.OrderByDescending(marketData => marketData.Date)
				.FirstOrDefaultAsync();

			result[currencyPair] = latestMarketData;
		}

		return result;
	}

	public async Task<Dictionary<CurrencyPair, List<MarketData?>>> GetMarketDataForCurrenciesByDateAsync(
		IEnumerable<CurrencyPair> currencyPairs, List<DateTime> requiredDates)
	{
		var result = new Dictionary<CurrencyPair, List<MarketData?>>();
		
		var requiredDateTimes = requiredDates
			.Select(d => d.Date.AddHours(d.Hour))
			.Distinct()
			.ToList();

		var marketDataList = await _context.MarketData
			.Where(marketData => 
				requiredDateTimes.Contains(marketData.Date.Date.AddHours(marketData.Date.Hour)))
			.ToListAsync();

		foreach (var currencyPair in currencyPairs)
		{
			var marketDataListByDates = new List<MarketData?>();

			var data = marketDataList
				.Where(marketData => marketData.From == currencyPair.From && marketData.To == currencyPair.To)
				.OrderByDescending(marketData => marketData.Date)
				.ToList();

			marketDataListByDates.AddRange(data);

			result[currencyPair] = marketDataListByDates;
		}

		return result;
	}

	public async Task<decimal> GetLastUsdToRubRateAsync(
		CurrencyCode from = CurrencyCode.USD,
		CurrencyCode to = CurrencyCode.RUB)
	{
		var usdToRubRate = await _context.MarketData
			.Where(marketData => marketData.From == from && marketData.To == to)
			.OrderByDescending(marketData => marketData.Date)
			.Select(marketData => marketData.Price)
			.FirstOrDefaultAsync();
		return usdToRubRate;
	}

	public async Task<Dictionary<DateOnly, decimal>> GetUsdToRubRatesByDateAsync(
		List<DateTime> payoutsDates,
		CurrencyCode from = CurrencyCode.USD,
		CurrencyCode to = CurrencyCode.RUB)
	{
		var usdToRubRatesByDate = new Dictionary<DateOnly, decimal>();
		
		var marketDataList = await _context.MarketData
			.Where(marketData =>
				marketData.From == from &&
				marketData.To == to &&
				payoutsDates.Select(d => d.Date)
					.Any(date => marketData.Date.Date <= date))
			.OrderByDescending(marketData => marketData.Date)
			.ToListAsync();

		foreach (var date in payoutsDates)
		{
			var usdToRubRate = marketDataList
				.Where(marketData => marketData.Date.Date <= date.Date)
				.OrderByDescending(marketData => marketData.Date)
				.Select(marketData => marketData.Price)
				.FirstOrDefault();

			usdToRubRatesByDate[DateOnly.FromDateTime(date.Date)] = usdToRubRate;
		}

		return usdToRubRatesByDate;
	}
}