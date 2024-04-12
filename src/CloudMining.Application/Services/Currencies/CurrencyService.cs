using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudMining.Application.Models.Currencies;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;
using CloudMining.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;


namespace CloudMining.Application.Services.Currencies
{
    public sealed class CurrencyService : ICurrencyService
    {
        private readonly CloudMiningContext _context;

        public CurrencyService(CloudMiningContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(CurrencyDto currency)
        {
            var newCurrency = new Currency
            {
                Caption = currency.Name,
                Code = currency.Code,
                Precision = currency.Precision
            };

            await _context.Currencies.AddAsync(newCurrency).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Currency> GetCurrencyByCodeAsync(CurrencyCode code)
        {
            var foundCurrency = await _context.Currencies
                .FirstOrDefaultAsync(c => c.Code == code)
                .ConfigureAwait(false);

            return foundCurrency;
        }
    }
}
