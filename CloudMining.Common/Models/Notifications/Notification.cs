using CloudMining.Common.Models.Base;

namespace CloudMining.Common.Models.Notifications;

public class Notification : Entity
{
	public Guid UserId { get; set; }
	public string Message { get; set; }
	public NotificationMethod Method { get; set; }
	public string UserIdentifier { get; set; }
}