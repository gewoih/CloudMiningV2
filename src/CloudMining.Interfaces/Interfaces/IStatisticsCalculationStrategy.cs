using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.DTO.Payments.Deposits;
using CloudMining.Interfaces.DTO.Statistics;
using CloudMining.Interfaces.DTO.Users;

namespace CloudMining.Interfaces.Interfaces;

public interface IStatisticsCalculationStrategy
{
	Task<List<StatisticsDto>> GetStatisticsAsync(
		List<ShareablePayment> payoutsList,
		List<ShareablePayment> electricityExpenseList,
		IEnumerable<CurrencyPair> uniqueCurrencyPairs,
		List<UserDto> userDtosList,
		Dictionary<Guid,List<DepositDto>> usersDeposits);
}