using CloudMining.Common.Models.Notifications;

namespace Modules.Notifications.Contracts.Interfaces;

public interface INotificationService
{
	Task<Notification?> SendAsync(Notification notification);
}