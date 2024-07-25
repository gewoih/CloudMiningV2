using CloudMining.Domain.Models;

namespace Modules.Notifications.Domain.Models;

public class Notification : Entity
{
	public Guid UserId { get; set; }
	public string Message { get; set; }
	public NotificationMethod Method { get; set; }
	public string UserIdentifier { get; set; }
}