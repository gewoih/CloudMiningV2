using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Base;
using CloudMining.Domain.Models.Identity;

namespace CloudMining.Domain.Models.Payments.Shareable
{
	public class PaymentShare : Entity
	{
		public User User { get; set; }
		public Guid UserId { get; set; }
		public ShareablePayment ShareablePayment { get; set; }
		public Guid ShareablePaymentId { get; set; }
		public DateTime Date { get; set; }
		public decimal Amount { get; set; }
		public decimal Share { get; set; }
		public ShareStatus Status { get; set; }
	}
}
