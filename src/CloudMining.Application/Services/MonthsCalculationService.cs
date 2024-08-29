using CloudMining.Infrastructure.Settings;
using CloudMining.Interfaces.Interfaces;
using Microsoft.Extensions.Options;

namespace CloudMining.Application.Services;

public class MonthsCalculationService : IMonthsCalculationService
{
	private readonly DateOnly _projectStartDate;

	public MonthsCalculationService(IOptions<ProjectInformationSettings> projectInformation)
	{
		_projectStartDate = projectInformation.Value.ProjectStartDate;
	}
	
	public int CalculateSinceProjectStart()
	{
		var currentDate = DateTime.UtcNow;
		
		var totalMonths = (currentDate.Year - _projectStartDate.Year) * 12 + currentDate.Month -
		                  _projectStartDate.Month;
		if (currentDate.Day < _projectStartDate.Day)
			totalMonths--;

		return totalMonths;
	}
}