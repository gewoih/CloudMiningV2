using CloudMining.Domain.Models.Notifications;

namespace CloudMining.Interfaces.Interfaces;

public interface INotificationManagementService
{
	Task<Notification> AddAsync(Notification notification);
}