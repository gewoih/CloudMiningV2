using Microsoft.AspNetCore.Identity;

namespace CloudMining.Domain.Models.Identity
{
	public class User : IdentityUser<Guid>
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Patronymic { get; set; }
		public string AvatarPath { get; set; }
		public List<Deposit> Deposits { get; set; }
		public List<ShareChange> ShareChanges { get; set; }
	}
}
