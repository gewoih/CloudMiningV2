using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudMining.Application.Models.Currencies;
using CloudMining.Domain.Models;
using CloudMining.Infrastructure.Database;


namespace CloudMining.Application.Services.Currencies
{
    public sealed class CurrencyService : ICurrencyService
    {
        private readonly CloudMiningContext _context;

        public CurrencyService(CloudMiningContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(CurrencyDTO currency)
        {
            var newCurrency = new Currency()
            {
                Caption = currency.Caption,
                Code = currency.Code,
                Precision = currency.Precision
            };
            await _context.Currencies.AddAsync(newCurrency).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
