using CloudMining.Domain.Models.Base;

namespace CloudMining.Domain.Models.Notifications;

public class Notification : Entity
{
	public Guid UserId { get; set; }
	public string Content { get; set; }
	public bool IsSent { get; set; }
}