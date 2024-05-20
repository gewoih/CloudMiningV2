using CloudMining.Application.DTO.Payments.Deposits;
using CloudMining.Application.Models.Payments.Deposits;
using CloudMining.Domain.Models;
using CloudMining.Domain.Models.Payments;

namespace CloudMining.Application.Services.Deposits
{
	public interface IDepositService
	{
		Task<Deposit> AddDepositAndRecalculateShares(CreateDepositDto depositDto);
	}
}