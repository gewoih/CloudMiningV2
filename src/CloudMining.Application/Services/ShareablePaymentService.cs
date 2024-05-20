using CloudMining.Application.Services.MassTransit.Events;
using CloudMining.Contracts.DTO.Payments;
using CloudMining.Contracts.Interfaces;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Infrastructure.Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services
{
	public sealed class ShareablePaymentService : IShareablePaymentService
	{
		private readonly CloudMiningContext _context;
		private readonly ICurrencyService _currencyService;
		private readonly IShareService _shareService;
		private readonly IUserService _userService;
		private readonly IPublishEndpoint _publishEndpoint;

		public ShareablePaymentService(CloudMiningContext context, 
			ICurrencyService currencyService, 
			IShareService shareService, 
			IUserService userService, 
			IPublishEndpoint publishEndpoint)
		{
			_context = context;
			_currencyService = currencyService;
			_shareService = shareService;
			_userService = userService;
			_publishEndpoint = publishEndpoint;
		}

		public async Task<int> GetUserPaymentsCount(PaymentType? paymentType = null)
		{
			var currentUserId = _userService.GetCurrentUserId();
			if (currentUserId == null)
				return 0;

			var paymentsQuery = _context.ShareablePayments.AsQueryable();
			if (paymentType != null)
				paymentsQuery = paymentsQuery.Where(payment => payment.Type == paymentType);

			var paymentsCount = await paymentsQuery.Where(payment =>
				payment.PaymentShares
					.Any(paymentShare => paymentShare.UserId == currentUserId))
				.CountAsync();

			return paymentsCount;
		}

		public async Task<ShareablePayment?> CreateAsync(CreatePaymentDto createPaymentDto)
		{
			var foundCurrency = await _currencyService.GetAsync(createPaymentDto.CurrencyCode);
			if (foundCurrency is null)
				return null;

			var usersPaymentShares = 
				await _shareService.CreatePaymentShares(createPaymentDto.Amount, foundCurrency, createPaymentDto.Date);
			
			//TODO: Mapper?
			var newPayment = new ShareablePayment
			{
				Amount = createPaymentDto.Amount,
				Caption = createPaymentDto.Caption,
				CurrencyId = foundCurrency.Id,
				Type = createPaymentDto.PaymentType,
				PaymentShares = usersPaymentShares,
				Date = createPaymentDto.Date.ToUniversalTime()
			};
			
			await _context.ShareablePayments.AddAsync(newPayment).ConfigureAwait(false);
			await _publishEndpoint.Publish(new ShareablePaymentCreated { Payment = newPayment });

			await _context.SaveChangesAsync().ConfigureAwait(false);

			return newPayment;
		}

		public async Task<DateTime> GetLatestPaymentDateAsync(PaymentType paymentType)
		{
			var latestPaymentDate = await _context.ShareablePayments
				.Where(payment => payment.Type == paymentType)
				.OrderByDescending(payment => payment.Date)
				.Select(payment => payment.Date)
				.FirstOrDefaultAsync();

			return latestPaymentDate;
		}

		public async Task<List<PaymentShare>> GetPaymentShares(Guid paymentId)
		{
			var currentUserId = _userService.GetCurrentUserId();
			var isCurrentUserAdmin = _userService.IsCurrentUserAdmin();

			//TODO: Получать User (ФИО) не из БД, а из UserService
			var userPaymentSharesQuery = _context.PaymentShares
				.Include(paymentShare => paymentShare.User)
				.Where(paymentShare => paymentShare.ShareablePaymentId == paymentId);
			
			if (!isCurrentUserAdmin)
				userPaymentSharesQuery = userPaymentSharesQuery.Where(paymentShare => paymentShare.UserId == currentUserId);

			var userPaymentShares = await userPaymentSharesQuery.ToListAsync();
			return userPaymentShares;
		}

		public async Task<List<ShareablePayment>> GetAsync(int skip, int take, bool withShares = false, PaymentType? paymentType = null)
		{
			var currentUserId = _userService.GetCurrentUserId();
			if (currentUserId == null)
				return [];
			
			var paymentsQuery = _context.ShareablePayments
				.Include(payment => payment.PaymentShares)
				.AsQueryable();

			if (withShares)
				paymentsQuery = paymentsQuery.Include(payment => payment.PaymentShares);
			
			if (paymentType != null)
				paymentsQuery = paymentsQuery.Where(payment => payment.Type == paymentType);
			
			//TODO: Необходимо забирать только те PaymentShare, которые относятся к пользователю. Он не должен видеть чужие доли.
			var isCurrentUserAdmin = _userService.IsCurrentUserAdmin();
			if (!isCurrentUserAdmin)
			{
				paymentsQuery = paymentsQuery.Where(payment =>
				payment.PaymentShares.Any(paymentShare => paymentShare.UserId == currentUserId));
			}

			var payments = await paymentsQuery
				.OrderByDescending(payment => payment.Date)
				.Skip(skip)
				.Take(take)
				.ToListAsync();
			
			return payments;
		}
	}
}
