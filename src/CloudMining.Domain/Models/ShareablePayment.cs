using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Base;

namespace CloudMining.Domain.Models
{
	public class ShareablePayment : Payment
	{
		public DateTime Date { get; set; }
		public PaymentType Type { get; set; }
		public bool IsCompleted { get; set; }
		public List<PaymentShare> PaymentShares { get; set; }
	}
}
