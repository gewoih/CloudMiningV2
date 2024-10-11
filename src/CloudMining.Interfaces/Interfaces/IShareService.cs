using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Domain.Models.Shares;
using CloudMining.Interfaces.DTO.Payments;

namespace CloudMining.Interfaces.Interfaces;

public interface IShareService
{
	decimal CalculateUserShare(List<ShareChange> shareChanges);
	Task<List<ShareChange>> GetUpdatedUsersSharesAsync(Dictionary<Guid, decimal> usersDeposits, DateTime newDepositDate);
	Task<List<PaymentShare>> CreatePaymentShares(CreatePaymentDto paymentDto, Currency currency);
}