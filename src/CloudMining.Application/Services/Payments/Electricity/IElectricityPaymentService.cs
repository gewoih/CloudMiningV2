using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudMining.Application.Models.Payments.Electricity;

namespace CloudMining.Application.Services.Payments.Electricity
{
    public interface IElectricityPaymentService
    {
        Task CreateAsync(ElectricityCredentials credentials);
    }
}
