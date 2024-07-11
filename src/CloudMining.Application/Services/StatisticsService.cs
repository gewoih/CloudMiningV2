using CloudMining.Domain.Enums;
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
    private readonly DateTime _projectStartDate;
    private readonly DateTime _currentDate;

    public StatisticsService(
        CloudMiningContext context,
        IShareablePaymentService shareablePaymentService
    )
    {
        _context = context;
        _projectStartDate = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        _currentDate = DateTime.UtcNow;
        _shareablePaymentService = shareablePaymentService;
    }

    public async Task<List<StatisticsDto>> GetStatisticsListAsync()
    {
        var payoutsList = await _shareablePaymentService.GetPayoutsAsync();

        var usdToRubRate = await _context.MarketData
            .Where(md => md.From == CurrencyCode.USD)
            .OrderByDescending(md => md.Date)
            .Select(md => md.Price)
            .FirstOrDefaultAsync();

        var statisticsDtoList = new List<StatisticsDto>();


        foreach (var type in Enum.GetValues(typeof(IncomeType)))
        {
            if ((IncomeType)type == IncomeType.Hold)
            {
                var totalIncome = await GetTotalIncome(payoutsList, usdToRubRate);
                var monthlyIncome = GetMonthlyValue(totalIncome);
                var electricityExpense = await GetElectricityExpense();
                var purchaseExpense = await GetPurchaseExpense();
                var totalExpense = electricityExpense + purchaseExpense;
                var totalProfit = totalIncome - totalExpense;
                var monthlyProfit = GetMonthlyValue(totalProfit);
                var paybackPercent = totalExpense != 0 ? totalProfit / totalExpense * 100 : 0;
                var chartDtos = await GetChartDtos();
            }
        }

        return statisticsDtoList;
    }

    private async Task<decimal> GetTotalIncome(IEnumerable<ShareablePayment> payoutsList, decimal usdToRubRate)
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

    private decimal GetMonthlyValue(decimal value)
    {
        var totalMonths = (_currentDate.Year - _projectStartDate.Year) * 12 + _currentDate.Month -
                          _projectStartDate.Month;
        var monthlyValue = value / totalMonths;
        return monthlyValue;
    }

    private async Task<decimal> GetElectricityExpense()
    {
        var electricityExpense = await _context.ShareablePayments
            .Where(payment => payment.Type == PaymentType.Electricity)
            .SumAsync(payment => payment.Amount);
        return electricityExpense;
    }

    private async Task<decimal> GetPurchaseExpense()
    {
        var purchaseExpense = await _context.ShareablePayments
            .Where(payment => payment.Type == PaymentType.Purchase)
            .SumAsync(payment => payment.Amount);
        return purchaseExpense;
    }

    private async Task<List<ChartDto>> GetChartDtos()
    {
        
    }
}