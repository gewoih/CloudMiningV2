using CloudMining.Application.DTO.NotificationSettings;
using CloudMining.Domain.Models.UserSettings;

namespace CloudMining.Application.Services.Notifications.Settings;

public interface INotificationSettingsService
{
	Task<NotificationSettings> GetUserSettingsAsync();
	Task<bool> UpdateUserSettingsAsync(NotificationSettingsDto settingsDto);
}