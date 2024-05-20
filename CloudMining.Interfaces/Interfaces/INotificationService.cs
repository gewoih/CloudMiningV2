using CloudMining.Domain.Models.Notifications;

namespace CloudMining.Interfaces.Interfaces;

public interface INotificationService
{
	Task<Notification> SendAsync(Notification notification);
}