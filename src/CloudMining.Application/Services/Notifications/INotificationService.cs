using CloudMining.Domain.Models.Notifications;

namespace CloudMining.Application.Services.Notifications;

public interface INotificationService
{
	Task<Notification> SendAsync(Notification notification);
}