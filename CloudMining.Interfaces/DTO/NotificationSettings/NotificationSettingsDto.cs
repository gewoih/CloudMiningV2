namespace CloudMining.Interfaces.DTO.NotificationSettings;

public record NotificationSettingsDto
{
	public bool IsTelegramNotificationsEnabled { get; init; }
	public bool NewPayoutNotification { get; init; }
	public bool NewElectricityPaymentNotification { get; init; }
	public bool NewPurchaseNotification { get; init; }
	public bool UnpaidElectricityPaymentReminder { get; init; }
	public bool UnpaidPurchasePaymentReminder { get; init; }
}