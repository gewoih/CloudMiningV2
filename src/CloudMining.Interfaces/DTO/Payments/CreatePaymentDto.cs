using CloudMining.Domain.Enums;

namespace CloudMining.Interfaces.DTO.Payments;

public record CreatePaymentDto(
	string? Caption,
	CurrencyCode CurrencyCode,
	PaymentType PaymentType,
	DateTime Date,
	decimal Amount);