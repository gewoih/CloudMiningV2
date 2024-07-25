namespace Modules.Notifications.Domain.Models;

public sealed class TelegramNotification : Notification
{
	public long TelegramUserId { get; set; }
	public string Message { get; set; }
}