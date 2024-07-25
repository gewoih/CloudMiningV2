using CloudMining.Common.Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modules.Currencies.Contracts.Interfaces;
using Modules.Notifications.Application.Services.MassTransit.Events;
using Modules.Payments.Contracts.DTO;
using Modules.Payments.Contracts.Interfaces;
using Modules.Payments.Domain.Enums;
using Modules.Payments.Domain.Models;
using Modules.Users.Contracts.Interfaces;

namespace Modules.Payments.Application.Services;

public sealed class ShareablePaymentService : IShareablePaymentService
{
	private readonly CloudMiningContext _context;
	private readonly ICurrencyService _currencyService;
	private readonly ICurrentUserService _currentUserService;
	private readonly IPublishEndpoint _publishEndpoint;
	private readonly IShareService _shareService;

	public ShareablePaymentService(CloudMiningContext context,
		ICurrencyService currencyService,
		IShareService shareService,
		ICurrentUserService currentUserService,
		IPublishEndpoint publishEndpoint)
	{
		_context = context;
		_currencyService = currencyService;
		_shareService = shareService;
		_currentUserService = currentUserService;
		_publishEndpoint = publishEndpoint;
	}

	public async Task<int> GetUserPaymentsCount(PaymentType? paymentType = null)
	{
		var currentUserId = _currentUserService.GetCurrentUserId();
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
		await _publishEndpoint.Publish(new PaymentCreated { Payment = newPayment });

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
		var currentUserId = _currentUserService.GetCurrentUserId();
		var isCurrentUserAdmin = _currentUserService.IsCurrentUserAdmin();

		//TODO: Получать User (ФИО) не из БД, а из UserService
		var userPaymentSharesQuery = _context.PaymentShares
			//TODO: Починить
			//.Include(paymentShare => paymentShare.User)
			.Where(paymentShare => paymentShare.ShareablePaymentId == paymentId);

		if (!isCurrentUserAdmin)
			userPaymentSharesQuery = userPaymentSharesQuery.Where(paymentShare => paymentShare.UserId == currentUserId);

		var userPaymentShares = await userPaymentSharesQuery.ToListAsync();
		return userPaymentShares;
	}

	public async Task<bool> CompletePaymentShareAsync(Guid paymentShareId)
	{
		var paymentShare = await _context.PaymentShares.FindAsync(paymentShareId);
		if (paymentShare == null)
			return false;

		var isCurrentUserAdmin = _currentUserService.IsCurrentUserAdmin();
		paymentShare.Status = isCurrentUserAdmin ? ShareStatus.Completed : ShareStatus.Pending;
		await _context.SaveChangesAsync().ConfigureAwait(false);

		return true;
	}

	public async Task<List<ShareablePayment>> GetAsync(int skip, int take, PaymentType? paymentType = null)
	{
		var currentUserId = _currentUserService.GetCurrentUserId();
		if (currentUserId == null)
			return [];

		var paymentsQuery = _context.ShareablePayments
			.Include(payment => payment.PaymentShares)
			.Include(payment => payment.Currency)
			.AsQueryable();

		if (paymentType != null)
			paymentsQuery = paymentsQuery.Where(payment => payment.Type == paymentType);

		//TODO: Необходимо забирать только те PaymentShare, которые относятся к пользователю. Он не должен видеть чужие доли.
		var isCurrentUserAdmin = _currentUserService.IsCurrentUserAdmin();
		if (!isCurrentUserAdmin)
			paymentsQuery = paymentsQuery.Where(payment =>
				payment.PaymentShares.Any(paymentShare => paymentShare.UserId == currentUserId));

		var payments = await paymentsQuery
			.OrderByDescending(payment => payment.Date)
			.Skip(skip)
			.Take(take)
			.ToListAsync();

		return payments;
	}
}