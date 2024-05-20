namespace CloudMining.Interfaces.DTO.Users
{
	public sealed class RegisterDto
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Patronymic { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
	}
}
