﻿using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Interfaces.DTO.Payments;

namespace CloudMining.Interfaces.Interfaces;

public interface IShareablePaymentService
{
	Task<List<ShareablePayment>> GetAsync(int skip = 0, int take = int.MaxValue, List<PaymentType>? paymentTypes = null,
		bool adminCheck = true);

	Task<int> GetUserPaymentsCount(PaymentType? paymentType = null);
	Task<ShareablePayment?> CreateAsync(CreatePaymentDto createPaymentDto);
	Task<DateTime> GetLatestPaymentDateAsync(PaymentType paymentType);
	Task<List<PaymentShare>> GetPaymentShares(Guid paymentId);
	Task<bool> CompletePaymentShareAsync(Guid paymentShareId);
}