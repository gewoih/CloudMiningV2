using System.Security;

namespace CloudMining.Application.Models.Users
{
	public sealed class LoginCredentials
	{
		public string Email { get; set; }
		public string Password { get; set; }
	}
}
