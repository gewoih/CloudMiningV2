using CloudMining.Domain.Models.UserSettings;
using CloudMining.Interfaces.DTO.NotificationSettings;

namespace CloudMining.Interfaces.Interfaces;

public interface INotificationSettingsService
{
	Task<NotificationSettings?> GetUserSettingsAsync(Guid userId);
	Task<NotificationSettings> UpdateUserSettingsAsync(Guid userId, NotificationSettingsDto notificationSettingsDto);
}