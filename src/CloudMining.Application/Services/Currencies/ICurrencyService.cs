using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;
using CloudMining.Domain.Models.Currencies;

namespace CloudMining.Application.Services.Currencies
{
    public interface ICurrencyService
    {
        Task<Currency?> GetAsync(CurrencyCode code);
        Task<Guid> GetIdAsync(CurrencyCode code);
    }
}
