using CloudMining.Application.Models.Payments.Electricity;
using CloudMining.Application.Services.ShareChanges;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;
using CloudMining.Infrastructure.Database;

namespace CloudMining.Application.Services.Payments.Electricity
{
    public sealed class ElectricityPaymentService : IElectricityPaymentService
    {
        private readonly CloudMiningContext _context;
        private readonly IShareService _shareService;

        public ElectricityPaymentService(CloudMiningContext context, IShareService shareService)
        {
	        _context = context;
	        _shareService = shareService;
        }

        public async Task<ShareablePayment> CreateAsync(CreateElectricityPaymentDto paymentDto)
        {
            //TODO: Взять currency из CurrencyService
	        var usersPaymentShares = await _shareService.CreatePaymentShares(paymentDto.Amount, new(), paymentDto.CreatedDate);
            var newPayment = new ShareablePayment
            {
                Amount = paymentDto.Amount,
                //Currency = ,
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
