using CloudMining.Common.Database;
using CloudMining.Common.Models.Currencies;
using Microsoft.EntityFrameworkCore;
using Modules.Currencies.Contracts.Interfaces;
using Modules.Currencies.Domain.Enums;

namespace Modules.Currencies.Application.Services;

public sealed class CurrencyService : ICurrencyService
{
	private readonly CloudMiningContext _context;

	public CurrencyService(CloudMiningContext context)
	{
		_context = context;
	}

	public async Task<Currency?> GetAsync(CurrencyCode code)
	{
		var foundCurrency = await _context.Currencies
			.FirstOrDefaultAsync(currency => currency.Code == code)
			.ConfigureAwait(false);

		return foundCurrency;
	}

	public async Task<Guid> GetIdAsync(CurrencyCode code)
	{
		var currencyId = await _context.Currencies
			.Where(currency => currency.Code == code)
			.Select(currency => currency.Id)
			.FirstOrDefaultAsync()
			.ConfigureAwait(false);

		return currencyId;
	}
}