using CloudMining.Application.DTO.Payments;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;

namespace CloudMining.Application.Services.Payments
{
    public interface IShareablePaymentService
    {
        Task<List<ShareablePayment>> GetAsync(PaymentType? paymentType = null, Guid? userId = null);
	    Task<ShareablePayment?> CreateAsync(CreateShareablePaymentDto createPaymentDto);
	    Task<DateTime> GetLatestPaymentDateAsync(PaymentType paymentType);
    }
}
