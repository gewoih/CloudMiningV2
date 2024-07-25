using CloudMining.Common.Models.UserSettings;
using Modules.Notifications.Contracts.DTO;

namespace Modules.Notifications.Contracts.Interfaces;

public interface INotificationSettingsService
{
	Task<NotificationSettings?> GetUserSettingsAsync(Guid userId);
	Task<NotificationSettings> UpdateUserSettingsAsync(Guid userId, NotificationSettingsDto notificationSettingsDto);
}