namespace CloudMining.Interfaces.DTO.NotificationSettings;

public sealed class NotificationSettingsDto
{
	public bool IsTelegramNotificationsEnabled { get; set; }
	public bool NewPayoutNotification { get; set; }
	public bool NewElectricityPaymentNotification { get; set; }
	public bool NewPurchaseNotification { get; set; }
	public bool UnpaidElectricityPaymentReminder { get; set; }
	public bool UnpaidPurchasePaymentReminder { get; set; }
}