using CloudMining.Application.Models.Payments.Electricity;
using CloudMining.Application.Services.Currencies;
using CloudMining.Application.Services.Shares;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;
using CloudMining.Infrastructure.Database;

namespace CloudMining.Application.Services.Payments.Electricity
{
	public sealed class ElectricityPaymentService : IElectricityPaymentService
    {
        private readonly CloudMiningContext _context;
        private readonly IShareService _shareService;
        private readonly ICurrencyService _currencyService;

        public ElectricityPaymentService(CloudMiningContext context, IShareService shareService, ICurrencyService currencyService)
        {
	        _context = context;
	        _shareService = shareService;
	        _currencyService = currencyService;
        }

        public async Task<ShareablePayment> CreateAsync(CreateElectricityPaymentDto paymentDto)
        {
	        var rubCurrency = await _currencyService.GetAsync(CurrencyCode.RUB);
	        var usersPaymentShares = await _shareService.CreatePaymentShares(paymentDto.Amount, rubCurrency, paymentDto.CreatedDate);
            var newPayment = new ShareablePayment
            {
                Amount = paymentDto.Amount,
                CurrencyId = rubCurrency.Id,
                Type = PaymentType.Electricity,
                IsCompleted = false,
                PaymentShares = usersPaymentShares
			};

            await _context.ShareablePayments.AddAsync(newPayment).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            
            return newPayment;
        }
    }
}
