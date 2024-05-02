using CloudMining.Domain.Models.Base;
using CloudMining.Domain.Models.Identity;

namespace CloudMining.Domain.Models
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
		public bool IsCompleted { get; set; }
	}
}
