using CloudMining.Domain.Models.UserSettings;
using CloudMining.Interfaces.DTO.NotificationSettings;

namespace CloudMining.Interfaces.Interfaces;

public interface INotificationSettingsService
{
	Task<NotificationSettings> GetUserSettingsAsync();
	Task<bool> UpdateUserSettingsAsync(NotificationSettingsDto settingsDto);
}