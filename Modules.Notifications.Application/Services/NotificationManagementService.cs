using CloudMining.Common.Database;
using CloudMining.Common.Models.Notifications;
using Modules.Notifications.Contracts.Interfaces;

namespace Modules.Notifications.Application.Services;

public sealed class NotificationManagementService : INotificationManagementService
{
	private readonly CloudMiningContext _context;

	public NotificationManagementService(CloudMiningContext context)
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