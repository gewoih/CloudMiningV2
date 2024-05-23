using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Payments;
using CloudMining.Interfaces.DTO.Payments.Status;

namespace CloudMining.Interfaces.Interfaces
{
    public interface IShareablePaymentService
    {
        Task<List<ShareablePayment>> GetAsync(int skip, int take, bool withShares = false, PaymentType? paymentType = null);
        Task<int> GetUserPaymentsCount(PaymentType? paymentType = null);
	    Task<ShareablePayment?> CreateAsync(CreatePaymentDto createPaymentDto);
	    Task<DateTime> GetLatestPaymentDateAsync(PaymentType paymentType);
        Task<List<PaymentShare>> GetPaymentShares(Guid paymentId);
        Task<bool> ChangeStatusAsync(ChangeStatusDto dto);
    }
}
