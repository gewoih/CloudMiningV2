using CloudMining.Application.DTO.Currencies;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;

namespace CloudMining.Application.Services.Currencies
{
    public interface ICurrencyService
    {
        Task<Currency> CreateAsync(CurrencyDto currency);
        Task<Currency?> GetAsync(CurrencyCode code);
        Task<Guid> GetIdAsync(CurrencyCode code);
    }
}
