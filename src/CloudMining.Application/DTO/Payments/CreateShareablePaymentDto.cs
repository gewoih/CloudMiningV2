using System.ComponentModel.DataAnnotations;
using CloudMining.Domain.Enums;

namespace CloudMining.Application.DTO.Payments
{
	public sealed class CreateShareablePaymentDto
	{
		public string? Caption { get; set; }
		public CurrencyCode CurrencyCode { get; set; }
		public PaymentType PaymentType { get; set; }
		public DateTime Date { get; set; }

		[Range(1, 1_000_000)]
		public decimal Amount { get; set; }

		public CreateShareablePaymentDto()
		{
		}

		public CreateShareablePaymentDto(string? caption, CurrencyCode currencyCode, PaymentType paymentType, DateTime dateTime, decimal amount)
		{
			Caption = caption;
			CurrencyCode = currencyCode;
			PaymentType = paymentType;
			Date = dateTime;
			Amount = amount;
		}
	}
}
