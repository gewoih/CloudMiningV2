namespace CloudMining.Common.Models.Notifications;

public sealed class TelegramNotification : Notification
{
	public long TelegramUserId { get; set; }
	public string Message { get; set; }
}