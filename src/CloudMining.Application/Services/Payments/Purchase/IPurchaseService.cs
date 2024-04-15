using CloudMining.Application.DTO.Payments.Electricity;
using CloudMining.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudMining.Application.DTO.Payments.Purchase;

namespace CloudMining.Application.Services.Payments.Purchase
{
    public interface IPurchaseService
    {
        Task<ShareablePayment> CreateAsync(CreatePurchaseDto paymentDto);
    }
}
