using CloudMining.Contracts.DTO.Payments;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Payments.Shareable;

namespace CloudMining.Contracts.Interfaces
{
    public interface IShareablePaymentService
    {
        Task<List<ShareablePayment>> GetAsync(int skip, int take, bool withShares = false, PaymentType? paymentType = null);
        Task<int> GetUserPaymentsCount(PaymentType? paymentType = null);
	    Task<ShareablePayment?> CreateAsync(CreatePaymentDto createPaymentDto);
	    Task<DateTime> GetLatestPaymentDateAsync(PaymentType paymentType);
        Task<List<PaymentShare>> GetPaymentShares(Guid paymentId);
    }
}
