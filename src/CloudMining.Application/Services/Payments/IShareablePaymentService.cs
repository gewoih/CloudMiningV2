using CloudMining.Application.DTO.Payments;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;

namespace CloudMining.Application.Services.Payments
{
    public interface IShareablePaymentService
    {
	    Task<ShareablePayment?> CreateAsync(CreateShareablePaymentDto createPaymentDto);
	    Task<DateTime> GetLatestPaymentDateAsync(PaymentType paymentType);
    }
}
