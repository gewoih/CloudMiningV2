using CloudMining.Domain.Models.UserSettings;

namespace CloudMining.Interfaces.Interfaces;

public interface INotificationSettingsService
{
	Task<NotificationSettings> GetUserSettingsAsync(Guid? userId = null);
}