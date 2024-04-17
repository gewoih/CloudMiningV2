using CloudMining.Application.DTO.Payments;
using CloudMining.Application.Services.Currencies;
using CloudMining.Application.Services.Shares;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;
using CloudMining.Infrastructure.Database;

namespace CloudMining.Application.Services.Payments
{
	public sealed class ShareablePaymentService : IShareablePaymentService
	{
		private readonly CloudMiningContext _context;
		private readonly ICurrencyService _currencyService;
		private readonly IShareService _shareService;

		public ShareablePaymentService(CloudMiningContext context, ICurrencyService currencyService, IShareService shareService)
		{
			_context = context;
			_currencyService = currencyService;
			_shareService = shareService;
		}

		public async Task<ShareablePayment?> CreateAsync(CreateShareablePaymentDto createPaymentDto)
		{
			var rubCurrency = await _currencyService.GetAsync(createPaymentDto.CurrencyCode);
			if (rubCurrency is null)
				return null;

			var usersPaymentShares = 
				await _shareService.CreatePaymentShares(createPaymentDto.Amount, rubCurrency, createPaymentDto.Date);
			
			var newPayment = new ShareablePayment
			{
				Amount = createPaymentDto.Amount,
				Caption = createPaymentDto.Caption,
				CurrencyId = rubCurrency.Id,
				Type = createPaymentDto.PaymentType,
				PaymentShares = usersPaymentShares
			};
			
			await _context.ShareablePayments.AddAsync(newPayment).ConfigureAwait(false);
			await _context.SaveChangesAsync().ConfigureAwait(false);

			return newPayment;
		}
	}
}
