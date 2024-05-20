using CloudMining.Domain.Models.Base;

namespace CloudMining.Domain.Models.Notifications;

public abstract class Notification : Entity
{
	public Guid UserId { get; set; }
	public bool IsSent { get; set; }
}