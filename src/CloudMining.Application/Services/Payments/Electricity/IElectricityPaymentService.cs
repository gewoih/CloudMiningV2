using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMining.Application.Services.Payments.Electricity
{
    public interface IElectricityPaymentService
    {
        Task CreateAsync();
    }
}
