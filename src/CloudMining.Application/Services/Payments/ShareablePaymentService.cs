﻿using CloudMining.Application.DTO.Payments;
using CloudMining.Application.Services.Currencies;
using CloudMining.Application.Services.Shares;
using CloudMining.Application.Services.Users;
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
		private readonly IUserService _userService;

		public ShareablePaymentService(CloudMiningContext context, 
			ICurrencyService currencyService, 
			IShareService shareService, 
			IUserService userService)
		{
			_context = context;
			_currencyService = currencyService;
			_shareService = shareService;
			_userService = userService;
		}

		public async Task<int> GetTotalRecords(PaymentType? paymentType = null)
		{
			var currentUserId = _userService.GetCurrentUserId();
			if (currentUserId == null)
				return 0;

			var paymentsQuery = _context.ShareablePayments
				.Include(payment => payment.PaymentShares)
				.AsQueryable();

			if (paymentType != null)
				paymentsQuery = paymentsQuery.Where(payment => payment.Type == paymentType);

			//TODO: Необходимо забирать только те PaymentShare, которые относятся к пользователю. Он не должен видеть чужие доли.
			paymentsQuery = paymentsQuery.Where(payment =>
				payment.PaymentShares.Any(paymentShare => paymentShare.UserId == currentUserId));
			var totalRecords = await paymentsQuery.CountAsync();
			return totalRecords;
		}

		public async Task<ShareablePayment?> CreateAsync(CreatePaymentDto createPaymentDto)
		{
			var foundCurrency = await _currencyService.GetAsync(createPaymentDto.CurrencyCode);
			if (foundCurrency is null)
				return null;

			var usersPaymentShares = 
				await _shareService.CreatePaymentShares(createPaymentDto.Amount, foundCurrency, createPaymentDto.Date);
			
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
			
			//TODO: Получать User (ФИО) не из БД, а из UserService
			var userPaymentShares = await _context.PaymentShares
				.Include(paymentShare => paymentShare.User)
				.Where(paymentShare => paymentShare.ShareablePaymentId == paymentId && 
				                       paymentShare.UserId == currentUserId)
				.ToListAsync();

			return userPaymentShares;
		}

		public async Task<List<ShareablePayment>> GetAsync(int skip, int take, PaymentType? paymentType = null)
		{
			var currentUserId = _userService.GetCurrentUserId();
			if (currentUserId == null)
				return [];

			var paymentsQuery = _context.ShareablePayments
				.Include(payment => payment.PaymentShares)
				.AsQueryable();

			if (paymentType != null)
				paymentsQuery = paymentsQuery.Where(payment => payment.Type == paymentType);

			//TODO: Необходимо забирать только те PaymentShare, которые относятся к пользователю. Он не должен видеть чужие доли.
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
}
