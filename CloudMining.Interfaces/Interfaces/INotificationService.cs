using CloudMining.Domain.Models.Notifications;

namespace CloudMining.Contracts.Interfaces;

public interface INotificationService
{
	Task<Notification> SendAsync(Notification notification);
}