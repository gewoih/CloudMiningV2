using CloudMining.Application.Models.Currencies;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;
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
        Task<Currency> GetCurrencyByCodeAsync(CurrencyCode code);
    }
}
