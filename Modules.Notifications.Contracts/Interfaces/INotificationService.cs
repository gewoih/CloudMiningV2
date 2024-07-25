using Modules.Notifications.Domain.Models;

namespace Modules.Notifications.Contracts.Interfaces;

public interface INotificationService
{
	Task<Notification?> SendAsync(Notification notification);
}