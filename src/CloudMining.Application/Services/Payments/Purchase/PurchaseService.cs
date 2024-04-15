using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudMining.Application.DTO.Payments.Purchase;
using CloudMining.Application.Services.Currencies;
using CloudMining.Application.Services.Shares;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;
using CloudMining.Infrastructure.Database;

namespace CloudMining.Application.Services.Payments.Purchase
{
    public sealed class PurchaseService : IPurchaseService
    {
        private readonly CloudMiningContext _context;
        private readonly IShareService _shareService;
        private readonly ICurrencyService _currencyService;

        public PurchaseService(CloudMiningContext context, IShareService shareService, ICurrencyService currencyService)
        {
            _context = context;
            _shareService = shareService;
            _currencyService = currencyService;
        }
        public async Task<ShareablePayment> CreateAsync(CreatePurchaseDto paymentDto)
        {
            var rubCurrency = await _currencyService.GetAsync(CurrencyCode.RUB);
            var usersPaymentShares =
                await _shareService.CreatePaymentShares(paymentDto.Amount, rubCurrency, paymentDto.Date);
            var newPayment = new ShareablePayment
            {
                Amount = paymentDto.Amount,
                Caption = paymentDto.Purpose,
                CurrencyId = rubCurrency.Id,
                Type = PaymentType.Purchase,
                PaymentShares = usersPaymentShares
            };
            await _context.ShareablePayments.AddAsync(newPayment).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return newPayment;
        }
    }
}
