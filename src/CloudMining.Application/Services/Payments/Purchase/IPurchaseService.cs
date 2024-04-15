using CloudMining.Domain.Models;
using CloudMining.Application.DTO.Payments.Purchase;

namespace CloudMining.Application.Services.Payments.Purchase
{
    public interface IPurchaseService
    {
        Task<ShareablePayment> CreateAsync(CreatePurchaseDto paymentDto);
    }
}
