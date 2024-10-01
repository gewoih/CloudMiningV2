using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.DTO.Statistics;

namespace CloudMining.Interfaces.Interfaces;

public interface IStatisticsHelper
{
	List<CurrencyPair> GetUniqueCurrencyPairs(IEnumerable<ShareablePayment> payouts);
	List<Expense> GetExpenses(List<ShareablePayment> payments, Guid? userId = null);
	List<MonthlyPriceBar> GetProfitsList(List<MonthlyPriceBar> incomes, IEnumerable<Expense> expenses);
	int CalculateMonthsSinceProjectStart();
	StatisticsDto GetGeneralStatisticsDto(List<StatisticsDto> statisticsDtoList);
}