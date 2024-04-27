using CloudMining.Application.DTO.Payments;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;

namespace CloudMining.Application.Services.Payments
{
    public interface IShareablePaymentService
    {
        Task<List<ShareablePayment>> GetAsync(PaymentType? paymentType = null);
	    Task<ShareablePayment?> CreateAsync(CreatePaymentDto createPaymentDto);
	    Task<DateTime> GetLatestPaymentDateAsync(PaymentType paymentType);
    }
}
