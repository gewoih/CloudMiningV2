﻿using CloudMining.Application.Mappings;
using CloudMining.Application.Services.MassTransit.Events;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Infrastructure.Database;
using CloudMining.Interfaces.DTO.Payments;
using CloudMining.Interfaces.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services;

public sealed class ShareablePaymentService : IShareablePaymentService
{
	private readonly CloudMiningContext _context;
	private readonly ICurrencyService _currencyService;
	private readonly ICurrentUserService _currentUserService;
	private readonly IPublishEndpoint _publishEndpoint;
	private readonly IShareService _shareService;
	private readonly IMapper<ShareablePayment, CreatePaymentDto> _shareablePaymentMapper;

	public ShareablePaymentService(CloudMiningContext context,
		ICurrencyService currencyService,
		IShareService shareService,
		ICurrentUserService currentUserService,
		IPublishEndpoint publishEndpoint,
		IMapper<ShareablePayment, CreatePaymentDto> shareablePaymentMapper)
	{
		_context = context;
		_currencyService = currencyService;
		_shareService = shareService;
		_currentUserService = currentUserService;
		_publishEndpoint = publishEndpoint;
		_shareablePaymentMapper = shareablePaymentMapper;
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
			await _shareService.CreatePaymentShares(createPaymentDto, foundCurrency);

		var newPayment = _shareablePaymentMapper.ToDomain(createPaymentDto);
		newPayment.CurrencyId = foundCurrency.Id;
		newPayment.PaymentShares = usersPaymentShares;

		await _context.ShareablePayments.AddAsync(newPayment).ConfigureAwait(false);
		await _publishEndpoint.Publish(new PaymentCreated { Payment = newPayment });

		await _context.SaveChangesAsync().ConfigureAwait(false);

		return newPayment;
	}

	public async Task<DateTime> GetLatestPaymentDateAsync(PaymentType paymentType)
	{
		var latestPaymentDate = await _context.ShareablePayments
			.Where(payment => payment.Type == paymentType && payment.Currency.Code != CurrencyCode.ETH)
			.OrderByDescending(payment => payment.Date)
			.Select(payment => payment.Date)
			.FirstOrDefaultAsync();

		return latestPaymentDate;
	}

	public async Task<List<PaymentShare>> GetPaymentShares(Guid paymentId)
	{
		var currentUserId = _currentUserService.GetCurrentUserId();
		var isCurrentUserAdmin = _currentUserService.IsCurrentUserAdmin();

		var userPaymentSharesQuery = _context.PaymentShares
			.Include(paymentShare => paymentShare.User)
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

	public async Task<List<ShareablePayment>> GetAsync(
		int skip = 0,
		int take = int.MaxValue,
		List<PaymentType>? paymentTypes = null,
		bool adminCheck = true)
	{
		var currentUserId = _currentUserService.GetCurrentUserId();
		if (currentUserId == null)
			return [];

		var paymentsQuery = _context.ShareablePayments
			.Include(payment => payment.Currency)
			.AsQueryable();

		if (paymentTypes != null)
			paymentsQuery = paymentsQuery.Where(payment => paymentTypes.Contains(payment.Type));

		paymentsQuery = paymentsQuery.Include(payment => payment.PaymentShares);

		if (adminCheck)
		{
			var isCurrentUserAdmin = _currentUserService.IsCurrentUserAdmin();
			if (!isCurrentUserAdmin)
			{
				paymentsQuery = paymentsQuery.Where(payment =>
					payment.PaymentShares.Any(paymentShare => paymentShare.UserId == currentUserId));
			}
		}

		var payments = await paymentsQuery
			.OrderByDescending(payment => payment.Date)
			.Skip(skip)
			.Take(take)
			.ToListAsync();

		return payments;
	}
}