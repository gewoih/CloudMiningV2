using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;

namespace CloudMining.Contracts.Interfaces
{
    public interface ICurrencyService
    {
        Task<Currency?> GetAsync(CurrencyCode code);
        Task<Guid> GetIdAsync(CurrencyCode code);
    }
}
