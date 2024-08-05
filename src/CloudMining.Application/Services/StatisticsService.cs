using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Infrastructure.Database;
using CloudMining.Infrastructure.Settings;
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
    private readonly IMarketDataService _marketDataService;

    public StatisticsService(CloudMiningContext context,
        IShareablePaymentService shareablePaymentService,
        IMarketDataService marketDataService)
    {
        _context = context;
        _shareablePaymentService = shareablePaymentService;
        _marketDataService = marketDataService;
        _monthsSinceProjectStartDate = CalculateMonthsSinceProjectStart();
    }

    private int CalculateMonthsSinceProjectStart()
    {
        var totalMonths = (_currentDate.Year - _projectStartDate.Year) * 12 + _currentDate.Month -
                          _projectStartDate.Month;
        if (_currentDate.Day < _projectStartDate.Day)
            totalMonths--;

        return totalMonths;
    }

    public async Task<StatisticsDto> GetStatisticsAsync(StatisticsCalculationStrategy statisticsCalculationStrategy)
    {
        var payoutsList =
            await _shareablePaymentService.GetAsync(paymentType: PaymentType.Crypto, includePaymentShares: false);

        List<MonthlyPriceBar> incomes;

        //TODO: Реализовать паттерн стратегия
        if (statisticsCalculationStrategy == StatisticsCalculationStrategy.Hold)
        {
            var usdToRubRate = await _context.MarketData
                .Where(marketData => marketData.From == CurrencyCode.USD && marketData.To == CurrencyCode.RUB)
                .OrderByDescending(marketData => marketData.Date)
                .Select(marketData => marketData.Price)
                .FirstOrDefaultAsync();

            incomes = await GetHoldIncomePriceBarsAsync(payoutsList, usdToRubRate);
        }
        else
            throw new NotImplementedException();

        var totalIncome = incomes.Sum(priceBar => priceBar.Value);
        var monthlyIncome = totalIncome / _monthsSinceProjectStartDate;
        var expenses = await GetExpensesAsync();
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

    private async Task<List<ShareablePayment>> GetExpensesAsync()
    {
        var expenses = await _context.ShareablePayments
            .Where(payment => payment.Type == PaymentType.Electricity || payment.Type == PaymentType.Purchase)
            .ToListAsync();

        return expenses;
    }

    private async Task<List<MonthlyPriceBar>> GetHoldIncomePriceBarsAsync(
        List<ShareablePayment> payouts, decimal usdToRubRate)
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
            var usdIncome = CalculateUsdIncome(incomeByCurrencies, currencyRates);
            var rubIncome = usdIncome * usdToRubRate;
            if (rubIncome == 0)
                continue;

            priceBars.Add(new MonthlyPriceBar(rubIncome, new DateOnly(processingDate.Year, processingDate.Month, 1)));
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

    private static decimal CalculateUsdIncome(IReadOnlyDictionary<CurrencyCode, decimal> incomeByCurrencies,
        Dictionary<CurrencyPair, MarketData?> currencyRates)
    {
        var usdIncome = 0m;

        foreach (var (currencyCode, incomeAmount) in incomeByCurrencies)
        {
            var requiredCurrencyPair = new CurrencyPair { From = currencyCode, To = CurrencyCode.USDT };

            if (currencyRates.TryGetValue(requiredCurrencyPair, out var marketData) && marketData != null)
            {
                usdIncome += incomeAmount * marketData.Price;
            }
        }

        return usdIncome;
    }

    private List<Expense> GetExpenses(List<ShareablePayment> payments)
    {
        var specificExpenseTypes = new[] { ExpenseType.OnlyElectricity, ExpenseType.OnlyPurchases };
        var expenseList = new List<Expense>();

        foreach (var expenseType in specificExpenseTypes)
        {
            List<MonthlyPriceBar> priceBars;

            if (expenseType == ExpenseType.OnlyElectricity)
            {
                priceBars = CalculateMonthlyExpenses(payments.Where(payment =>
                    payment.Type == PaymentType.Electricity));
            }
            else
            {
                priceBars = CalculateMonthlyExpenses(payments.Where(payment =>
                    payment.Type == PaymentType.Purchase));
            }

            expenseList.Add(new Expense(expenseType, priceBars));
        }

        var generalExpensePriceBars = CalculateGeneralMonthlyExpenses(expenseList);
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
            {
                priceBars.Add(new MonthlyPriceBar(totalAmount,
                    new DateOnly(processingDate.Year, processingDate.Month, 1)));
            }

            processingDate = processingDate.AddMonths(1);
        }

        return priceBars;
    }

    private static List<MonthlyPriceBar> CalculateGeneralMonthlyExpenses(IReadOnlyCollection<Expense> expenseList)
    {
        var electricityPriceBars = expenseList
            .Where(expense => expense.Type == ExpenseType.OnlyElectricity)
            .SelectMany(expense => expense.PriceBars)
            .ToList();

        var purchasePriceBars = expenseList
            .Where(expense => expense.Type == ExpenseType.OnlyPurchases)
            .SelectMany(expense => expense.PriceBars)
            .ToList();

        var generalExpensePriceBars = electricityPriceBars
            .Concat(purchasePriceBars)
            .GroupBy(priceBar => priceBar.Date)
            .Select(group => new MonthlyPriceBar(
                Value: group.Sum(priceBar => priceBar.Value),
                Date: group.Key
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