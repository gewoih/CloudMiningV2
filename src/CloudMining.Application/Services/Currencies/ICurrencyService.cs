using CloudMining.Application.Models.Currencies;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;

namespace CloudMining.Application.Services.Currencies
{
    public interface ICurrencyService
    {
        Task CreateAsync(CurrencyDto currency);
        Task<Currency> GetCurrencyByCodeAsync(CurrencyCode code);
    }
}
