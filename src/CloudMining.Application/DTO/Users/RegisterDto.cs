using System.ComponentModel.DataAnnotations;

namespace CloudMining.Application.DTO.Users
{
	public sealed class RegisterDto
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Patronymic { get; set; }

		[EmailAddress]
		public string Email { get; set; }

		public string Password { get; set; }
	}
}
