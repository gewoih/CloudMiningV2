namespace CloudMining.Application.Models.Users
{
	public sealed class RegisterCredentials
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Patronymic { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
	}
}
