using Modules.Notifications.Domain.Models;

namespace Modules.Notifications.Contracts.Interfaces;

public interface INotificationManagementService
{
	Task<Notification> AddAsync(Notification notification);
}