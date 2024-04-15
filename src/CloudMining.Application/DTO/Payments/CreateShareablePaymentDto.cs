using CloudMining.Domain.Enums;

namespace CloudMining.Application.DTO.Payments
{
	public sealed class CreateShareablePaymentDto
	{
		public string? Caption { get; set; }
		public PaymentType PaymentType { get; set; }
		public DateTime Date { get; set; }
		public decimal Amount { get; set; }
	}
}
