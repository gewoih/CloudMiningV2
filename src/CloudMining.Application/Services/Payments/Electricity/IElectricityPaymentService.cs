using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudMining.Application.DTO.Payments.Electricity;
using CloudMining.Domain.Models;

namespace CloudMining.Application.Services.Payments.Electricity
{
	public interface IElectricityPaymentService
    {
        Task<ShareablePayment> CreateAsync(CreateElectricityPaymentDto paymentDto);
    }
}
