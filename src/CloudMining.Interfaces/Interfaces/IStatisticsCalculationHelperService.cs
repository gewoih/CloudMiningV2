using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.DTO.Statistics;

namespace CloudMining.Interfaces.Interfaces;

public interface IStatisticsCalculationHelperService
{
	List<CurrencyPair> GetUniqueCurrencyPairs(IEnumerable<ShareablePayment> payouts);
	List<Expense> GetExpenses(List<ShareablePayment> payments);
	List<MonthlyPriceBar> GetProfitsList(List<MonthlyPriceBar> incomes, IEnumerable<Expense> expenses);
}