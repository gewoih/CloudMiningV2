using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Infrastructure.Database;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services
{
    public sealed class CurrencyService : ICurrencyService
    {
        private readonly CloudMiningContext _context;

        public CurrencyService(CloudMiningContext context)
        {
            _context = context;
        }

        public async Task<Currency> CreateAsync(CurrencyDto currency)
        {
            var newCurrency = new Currency
            {
                Caption = currency.Name,
                Code = currency.Code,
                Precision = currency.Precision
            };

            await _context.Currencies.AddAsync(newCurrency).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return newCurrency;
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
}
