using CloudMining.Application.DTO.Payments;
using CloudMining.Application.Services.Currencies;
using CloudMining.Application.Services.Shares;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;
using CloudMining.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

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

		public async Task<ShareablePayment> CreateAsync(CreateShareablePaymentDto createPaymentDto)
		{
			var rubCurrency = await _currencyService.GetAsync(CurrencyCode.RUB);
			var usersPaymentShares = 
				await _shareService.CreatePaymentShares(createPaymentDto.Amount, rubCurrency, createPaymentDto.Date);
			
			var newPayment = new ShareablePayment
			{
				Amount = createPaymentDto.Amount,
				Caption = createPaymentDto.Caption,
				CurrencyId = rubCurrency.Id,
				Type = createPaymentDto.PaymentType,
				PaymentShares = usersPaymentShares,
				Date = createPaymentDto.Date.ToUniversalTime()
			};
			
			await _context.ShareablePayments.AddAsync(newPayment).ConfigureAwait(false);
			await _context.SaveChangesAsync().ConfigureAwait(false);

			return newPayment;
		}

		public async Task<List<ShareablePayment>> GetAsync(PaymentType? paymentType = null, Guid? userId = null)
		{
			var paymentsQuery = _context.ShareablePayments
				.Include(payment => payment.PaymentShares)
				.AsQueryable();

			if (paymentType != null)
				paymentsQuery = paymentsQuery.Where(payment => payment.Type == paymentType);

			//TODO: Необходимо забирать только те PaymentShare, которые относятся к пользователю. Он не должен видеть чужие доли.
			if (userId != null)
				paymentsQuery = paymentsQuery.Where(payment =>
					payment.PaymentShares.Any(paymentShare => paymentShare.UserId == userId));

			var payments = await paymentsQuery.ToListAsync();
			return payments;
		}
	}
}
