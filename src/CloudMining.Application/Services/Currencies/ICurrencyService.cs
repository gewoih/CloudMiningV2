using CloudMining.Application.Models.Currencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMining.Application.Services.Currencies
{
    public interface ICurrencyService
    {
        Task CreateAsync(CurrencyDTO currency);
    }
}
