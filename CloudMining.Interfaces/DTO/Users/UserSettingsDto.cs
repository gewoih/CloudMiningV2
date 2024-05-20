using CloudMining.Interfaces.DTO.NotificationSettings;

namespace CloudMining.Interfaces.DTO.Users;

public class UserSettingsDto
{
	public string? TelegramUsername { get; set; }
	public NotificationSettingsDto? NotificationSettings { get; set; }
}