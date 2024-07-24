using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Infrastructure.Database;
using CloudMining.Interfaces.DTO.Statistics;
using CloudMining.Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services;

public class StatisticsService : IStatisticsService
{
    private readonly CloudMiningContext _context;
    private readonly IShareablePaymentService _shareablePaymentService;
    //TODO: Вынести в appsettings
    private readonly DateTime _projectStartDate = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private readonly DateTime _currentDate = DateTime.UtcNow;
    private readonly int _monthsSinceProjectStartDate;

    public StatisticsService(CloudMiningContext context, IShareablePaymentService shareablePaymentService)
    {
        _context = context;
        _shareablePaymentService = shareablePaymentService;
        _monthsSinceProjectStartDate = CalculateMonthsSinceProjectStart();
    }

    private int CalculateMonthsSinceProjectStart()
    {
        var totalMonths = (_currentDate.Year - _projectStartDate.Year) * 12 + _currentDate.Month - _projectStartDate.Month;
        if (_currentDate.Day < _projectStartDate.Day)
            totalMonths--;

        return totalMonths;
    }

    public async Task<StatisticsDto> GetStatisticsAsync(StatisticsCalculationStrategy statisticsCalculationStrategy)
    {
        var payoutsList =
            await _shareablePaymentService.GetAsync(paymentType: PaymentType.Crypto, includePaymentShares: false);
        
        decimal totalIncome;
        List<PriceBar> incomes;

        if (statisticsCalculationStrategy == StatisticsCalculationStrategy.Hold)
        {
            var usdToRubRate = await _context.MarketData
                .Where(marketData => marketData.From == CurrencyCode.USD && marketData.To == CurrencyCode.RUB)
                .OrderByDescending(marketData => marketData.Date)
                .Select(marketData => marketData.Price)
                .FirstOrDefaultAsync();

            totalIncome = await GetTotalHoldIncomeAsync(payoutsList, usdToRubRate);
            incomes = await GetHoldIncomesPriceBarListAsync(payoutsList, usdToRubRate);
        }
        else
            throw new NotImplementedException();

        var monthlyIncome = totalIncome / _monthsSinceProjectStartDate;
        var expenses = await GetExpensesAsync();
        var spentOnElectricity = expenses.Where(payment => payment.Type == PaymentType.Electricity).Sum(payment => payment.Amount);
        var spentOnPurchases = expenses.Where(payment => payment.Type == PaymentType.Purchase).Sum(payment => payment.Amount);
        var totalExpense = spentOnElectricity + spentOnPurchases;
        var totalProfit = totalIncome - totalExpense;
        var monthlyProfit = totalProfit / _monthsSinceProjectStartDate;
        var paybackPercent = totalExpense != 0 ? totalProfit / totalExpense * 100 : 0;
        var expensesList = await GetExpenseListAsync();
        var profits = GetProfitsList(incomes, expenses);
        
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

    private async Task<decimal> GetTotalHoldIncomeAsync(IEnumerable<ShareablePayment> payoutsList, decimal usdToRubRate)
    {
        var totalAmountByCurrencyCode = payoutsList
            .GroupBy(payout => payout.Currency.Code)
            .Select(group => new
            {
                CurrencyCode = group.Key,
                TotalAmount = group.Sum(payout => payout.Amount)
            })
            .ToList();

        var latestMarketDataByCurrencyCode = await _context.MarketData
            .Where(marketData => totalAmountByCurrencyCode
                .Select(payout => payout.CurrencyCode)
                .Contains(marketData.From))
            .GroupBy(marketData => marketData.From)
            .Select(group => group
                .OrderByDescending(marketData => marketData.Date)
                .First())
            .ToListAsync();

        var combinedPayoutsAndMarketData = totalAmountByCurrencyCode
            .Join(latestMarketDataByCurrencyCode,
                payout => payout.CurrencyCode,
                marketData => marketData.From,
                (payout, marketData) => new
                {
                    payout.CurrencyCode,
                    payout.TotalAmount,
                    marketData.Price
                })
            .ToList();

        var totalIncomeUsd = combinedPayoutsAndMarketData
            .Select(data => data.TotalAmount * data.Price)
            .Sum();

        var totalIncome = totalIncomeUsd * usdToRubRate;

        return totalIncome;
    }

    private async Task<List<ShareablePayment>> GetExpensesAsync()
    {
        var expenses = await _context.ShareablePayments
            .Where(payment => payment.Type == PaymentType.Electricity || payment.Type == PaymentType.Purchase)
            .ToListAsync();
            
        return expenses;
    }

    private async Task<List<PriceBar>> GetHoldIncomesPriceBarListAsync(IEnumerable<ShareablePayment> payoutsList,
        decimal usdToRubRate)
    {
        var priceBars = new List<PriceBar>();

        var monthlyPayouts = payoutsList
            .GroupBy(payout => new { payout.Date.Year, payout.Date.Month })
            .Select(group => new
            {
                group.Key.Year,
                group.Key.Month,
                Payouts = group.ToList()
            })
            .OrderBy(g => g.Year).ThenBy(g => g.Month)
            .ToList();

        var currentExchangeRates = await _context.MarketData
            .Where(md => md.Date <= _currentDate)
            .GroupBy(md => md.From)
            .Select(group => group.OrderByDescending(md => md.Date).First())
            .ToDictionaryAsync(md => md.From, md => md.Price);

        var date = _projectStartDate;

        while (date <= _currentDate)
        {
            var month = date.Month;
            var year = date.Year;

            var monthlyPayout = monthlyPayouts.Find(p => p.Year == year && p.Month == month);
            decimal totalIncomeRub = 0;

            if (monthlyPayout != null)
            {
                var totalIncomeUsd = monthlyPayout.Payouts
                    .GroupBy(payout => payout.Currency.Code)
                    .Select(group => new
                    {
                        CurrencyCode = group.Key,
                        TotalAmount = group.Sum(payout => payout.Amount)
                    })
                    .Select(payout =>
                        payout.TotalAmount * currentExchangeRates.GetValueOrDefault(payout.CurrencyCode, 1m))
                    .Sum();

                totalIncomeRub = totalIncomeUsd * usdToRubRate;
            }

            priceBars.Add(new PriceBar(totalIncomeRub, new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc)));

            date = date.AddMonths(1);
        }

        return priceBars;
    }

    private async Task<List<Expense>> GetExpenseListAsync()
    {
        var expenses = await _context.ShareablePayments
            .Where(payment => payment.Type == PaymentType.Electricity || payment.Type == PaymentType.Purchase)
            .ToListAsync();

        var monthlyExpenses = expenses
            .GroupBy(payment => new { payment.Type, payment.Date.Year, payment.Date.Month })
            .Select(group => new
            {
                group.Key.Type,
                group.Key.Year,
                group.Key.Month,
                TotalAmount = group.Sum(payment => payment.Amount)
            })
            .ToList();

        var expenseList = new List<Expense>();

        var specificExpenseTypes = new[] { ExpenseType.OnlyElectricity, ExpenseType.OnlyPurchases };

        foreach (var expenseType in specificExpenseTypes)
        {
            var priceBars = new List<PriceBar>();

            var date = _projectStartDate;
            while (date <= _currentDate)
            {
                var totalAmount = monthlyExpenses
                    .Where(e => e.Type == (PaymentType)expenseType &&
                                e.Year == date.Year &&
                                e.Month == date.Month)
                    .Sum(e => e.TotalAmount);

                priceBars.Add(new PriceBar(
                    totalAmount,
                    new DateTime(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Utc)));

                date = date.AddMonths(1);
            }

            expenseList.Add(new Expense(expenseType, priceBars));
        }

        var generalPriceBars = new List<PriceBar>();
        var generalDate = _projectStartDate;
        while (generalDate <= _currentDate)
        {
            var totalGeneralAmount = specificExpenseTypes
                .Select(expenseType => monthlyExpenses
                    .Where(e => e.Type == (PaymentType)expenseType &&
                                e.Year == generalDate.Year &&
                                e.Month == generalDate.Month)
                    .Sum(e => e.TotalAmount))
                .Sum();

            generalPriceBars.Add(new PriceBar(
                totalGeneralAmount,
                new DateTime(generalDate.Year, generalDate.Month, 1, 0, 0, 0, DateTimeKind.Utc)));

            generalDate = generalDate.AddMonths(1);
        }

        expenseList.Add(new Expense(ExpenseType.Total, generalPriceBars));

        return expenseList;
    }

    private static List<PriceBar> GetProfitsList(List<PriceBar> incomes, List<Expense> expenses)
    {
        var generalExpenses = expenses.Find(e => e.Type == ExpenseType.Total)?.PriceBars;

        if (generalExpenses == null)
        {
            return incomes;
        }

        var profits = new List<PriceBar>();

        foreach (var income in incomes)
        {
            var generalExpense =
                generalExpenses.Find(e => e.Date.Year == income.Date.Year && e.Date.Month == income.Date.Month);

            if (generalExpense == null) continue;
            var profitValue = income.Value - generalExpense.Value;

            profits.Add(income with { Value = profitValue });
        }

        return profits;
    }
}