using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Infrastructure.Database;
using CloudMining.Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services;

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