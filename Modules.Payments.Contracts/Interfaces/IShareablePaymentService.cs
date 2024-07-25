using CloudMining.Common.Models.Payments.Shareable;
using Modules.Payments.Contracts.DTO;
using Modules.Payments.Domain.Enums;

namespace Modules.Payments.Contracts.Interfaces;

public interface IShareablePaymentService
{
	Task<List<ShareablePayment>> GetAsync(int skip, int take, PaymentType? paymentType = null);
	Task<int> GetUserPaymentsCount(PaymentType? paymentType = null);
	Task<ShareablePayment?> CreateAsync(CreatePaymentDto createPaymentDto);
	Task<DateTime> GetLatestPaymentDateAsync(PaymentType paymentType);
	Task<List<PaymentShare>> GetPaymentShares(Guid paymentId);
	Task<bool> CompletePaymentShareAsync(Guid paymentShareId);
}