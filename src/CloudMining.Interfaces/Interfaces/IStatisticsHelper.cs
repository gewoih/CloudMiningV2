using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.DTO.Payments.Deposits;
using CloudMining.Interfaces.DTO.Statistics;
using CloudMining.Interfaces.DTO.Users;

namespace CloudMining.Interfaces.Interfaces;

public interface IStatisticsHelper
{
	List<CurrencyPair> GetUniqueCurrencyPairs(IEnumerable<ShareablePayment> payouts);
	Task<List<UserDto>> GetUserDtosAsync(bool withAdminCheck = false);
	List<StatisticsDto> GetStatisticsDtoList(
		Dictionary<UserDto,List<MonthlyPriceBar>> incomesPerUser,
		List<ShareablePayment> expenseList,
		Dictionary<Guid,List<DepositDto>>? usersDeposits);
}