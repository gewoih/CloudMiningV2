using Modules.Notifications.Contracts.Interfaces;
using Modules.Notifications.Domain.Models;
using Modules.Notifications.Infrastructure.Database;

namespace Modules.Notifications.Application.Services;

public sealed class NotificationManagementService : INotificationManagementService
{
	private readonly NotificationsContext _context;

	public NotificationManagementService(NotificationsContext context)
	{
		_context = context;
	}

	public async Task<Notification> AddAsync(Notification notification)
	{
		await _context.Notifications.AddAsync(notification);
		await _context.SaveChangesAsync().ConfigureAwait(false);

		return notification;
	}
}