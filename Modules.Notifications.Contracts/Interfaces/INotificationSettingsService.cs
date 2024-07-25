using Modules.Notifications.Contracts.DTO;
using Modules.Notifications.Domain.Models;

namespace Modules.Notifications.Contracts.Interfaces;

public interface INotificationSettingsService
{
	Task<NotificationSettings?> GetUserSettingsAsync(Guid userId);
	Task<NotificationSettings> UpdateUserSettingsAsync(Guid userId, NotificationSettingsDto notificationSettingsDto);
}