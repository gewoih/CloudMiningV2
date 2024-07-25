using CloudMining.Common.Models.Notifications;

namespace Modules.Notifications.Contracts.Interfaces;

public interface INotificationManagementService
{
	Task<Notification> AddAsync(Notification notification);
}