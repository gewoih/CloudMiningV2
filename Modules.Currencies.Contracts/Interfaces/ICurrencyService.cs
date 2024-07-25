using CloudMining.Common.Models.Currencies;
using Modules.Currencies.Domain.Enums;

namespace Modules.Currencies.Contracts.Interfaces;

public interface ICurrencyService
{
	Task<Currency?> GetAsync(CurrencyCode code);
	Task<Guid> GetIdAsync(CurrencyCode code);
}