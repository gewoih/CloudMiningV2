using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Statistics;
using CloudMining.Interfaces.Interfaces;

namespace CloudMining.Application.Services;

public class StatisticsService : IStatisticsService
{
	private readonly IStatisticsCalculationStrategyFactory _statisticsCalculationStrategyFactory;
	private readonly IShareablePaymentService _shareablePaymentService;
	private readonly IStatisticsHelper _statisticsHelper;
	private readonly IDepositService _depositService;

	public StatisticsService(IStatisticsCalculationStrategyFactory statisticsCalculationStrategyFactory,
		IShareablePaymentService shareablePaymentService,
		IStatisticsHelper statisticsHelper,
		IDepositService depositService)
	{
		_statisticsCalculationStrategyFactory = statisticsCalculationStrategyFactory;
		_shareablePaymentService = shareablePaymentService;
		_statisticsHelper = statisticsHelper;
		_depositService = depositService;
	}

	public async Task<List<StatisticsDto>> GetStatisticsAsync(StatisticsCalculationStrategy strategy)
	{
		var userDtosList = await _statisticsHelper.GetUserDtosAsync(withAdminCheck: true);
		var shareablePaymentList = await _shareablePaymentService.GetAsync(
			paymentTypes: [PaymentType.Crypto, PaymentType.Electricity], 
			adminCheck: false);
		var usersDeposits = await _depositService.GetDepositsPerUserAsync(userDtosList);
		
		var payoutsList = 
			shareablePaymentList.Where(payment => payment.Type == PaymentType.Crypto).ToList();
		var electricityExpenseList =
			shareablePaymentList.Where(payment => payment.Type == PaymentType.Electricity).ToList();
		var uniqueCurrencyPairs = _statisticsHelper.GetUniqueCurrencyPairs(payoutsList);

		var statisticsCalculationStrategy = _statisticsCalculationStrategyFactory.Create(strategy);
		var statisticsDtoList = await statisticsCalculationStrategy.GetStatisticsAsync(payoutsList, electricityExpenseList,
			uniqueCurrencyPairs, userDtosList, usersDeposits);
		return statisticsDtoList;
	}
}