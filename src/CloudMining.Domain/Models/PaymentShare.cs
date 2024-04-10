using CloudMining.Domain.Models.Base;
using CloudMining.Domain.Models.Identity;

namespace CloudMining.Domain.Models
{
	public class PaymentShare : Entity
	{
		public User User { get; set; }
		public decimal Amount { get; set; }
		public decimal Percent { get; set; }
		public bool IsCompleted { get; set; }
	}
}
