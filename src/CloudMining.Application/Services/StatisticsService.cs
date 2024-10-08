using CloudMining.Domain.Enums;
using CloudMining.Interfaces.DTO.Statistics;
using CloudMining.Interfaces.Interfaces;

namespace CloudMining.Application.Services;

public class StatisticsService : IStatisticsService
{
	private readonly IStatisticsCalculationStrategyFactory _statisticsCalculationStrategyFactory;
	private readonly IShareablePaymentService _shareablePaymentService;
	private readonly IStatisticsHelper _statisticsHelper;

	public StatisticsService(IStatisticsCalculationStrategyFactory statisticsCalculationStrategyFactory,
		IShareablePaymentService shareablePaymentService,
		IStatisticsHelper statisticsHelper)
	{
		_statisticsCalculationStrategyFactory = statisticsCalculationStrategyFactory;
		_shareablePaymentService = shareablePaymentService;
		_statisticsHelper = statisticsHelper;
	}

	public async Task<List<StatisticsDto>> GetStatisticsAsync(StatisticsCalculationStrategy strategy)
	{
		var payoutsList =
			await _shareablePaymentService.GetAsync(paymentTypes: [PaymentType.Crypto], adminCheck: false);
		var expenseList =
			await _shareablePaymentService.GetAsync(paymentTypes: [PaymentType.Electricity, PaymentType.Purchase],
				adminCheck: false);
		var uniqueCurrencyPairs = _statisticsHelper.GetUniqueCurrencyPairs(payoutsList);
		var userDtosList = await _statisticsHelper.GetUserDtosAsync(withAdminCheck: true);

		var statisticsCalculationStrategy = _statisticsCalculationStrategyFactory.Create(strategy);
		var statisticsDtoList = await statisticsCalculationStrategy.GetStatisticsAsync(payoutsList, expenseList,
			uniqueCurrencyPairs, userDtosList);
		return statisticsDtoList;
	}
}