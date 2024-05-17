namespace CloudMining.Application.DTO.Users;

public sealed class ChangePasswordDto
{
	public string CurrentPassword { get; set; }
	public string NewPassword { get; set; }
}