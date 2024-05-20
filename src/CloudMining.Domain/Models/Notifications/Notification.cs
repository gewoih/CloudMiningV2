using CloudMining.Domain.Models.Base;

namespace CloudMining.Domain.Models.Notifications;

public class Notification : Entity
{
	public Guid UserId { get; set; }
	public string Message { get; set; }
	public NotificationMethod Method { get; set; }
	public string UserIdentifier { get; set; }
}