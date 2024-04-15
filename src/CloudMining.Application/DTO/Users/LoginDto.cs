using System.ComponentModel.DataAnnotations;

namespace CloudMining.Application.DTO.Users
{
	public sealed class LoginDto
	{
		[EmailAddress]
		public string Email { get; set; }
		public string Password { get; set; }
	}
}
