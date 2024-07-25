using Modules.Currencies.Domain.Enums;
using Modules.Payments.Domain.Enums;

namespace Modules.Payments.Contracts.DTO;

public record CreatePaymentDto(
	string? Caption,
	CurrencyCode CurrencyCode,
	PaymentType PaymentType,
	DateTime Date,
	decimal Amount);