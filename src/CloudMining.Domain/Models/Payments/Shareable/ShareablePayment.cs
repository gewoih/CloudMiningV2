using CloudMining.Domain.Enums;

namespace CloudMining.Domain.Models.Payments.Shareable
{
	public class ShareablePayment : Payment
	{
		public DateTime Date { get; set; }
		public PaymentType Type { get; set; }
		public bool IsCompleted { get; set; }
		public List<PaymentShare> PaymentShares { get; set; }
	}
}
