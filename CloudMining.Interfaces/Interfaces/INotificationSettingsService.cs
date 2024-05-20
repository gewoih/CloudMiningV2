using CloudMining.Contracts.DTO.NotificationSettings;
using CloudMining.Domain.Models.UserSettings;

namespace CloudMining.Contracts.Interfaces;

public interface INotificationSettingsService
{
	Task<NotificationSettings> GetUserSettingsAsync();
	Task<bool> UpdateUserSettingsAsync(NotificationSettingsDto settingsDto);
}