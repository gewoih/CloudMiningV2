using Modules.Currencies.Domain.Enums;
using Modules.Currencies.Domain.Models;

namespace Modules.Currencies.Contracts.Interfaces;

public interface ICurrencyService
{
	Task<Currency?> GetAsync(CurrencyCode code);
	Task<Guid> GetIdAsync(CurrencyCode code);
}