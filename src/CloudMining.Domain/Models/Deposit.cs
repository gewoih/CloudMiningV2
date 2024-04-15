using CloudMining.Domain.Models.Base;
using CloudMining.Domain.Models.Identity;

namespace CloudMining.Domain.Models
{
	public class Deposit : Payment
	{
		public User User { get; set; }
		public Guid UserId { get; set; }
		public DateTime Date { get; set; }
		public List<ShareChange> ShareChanges { get; set; }
	}
}
