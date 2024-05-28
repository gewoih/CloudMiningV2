namespace CloudMining.Interfaces.DTO.NotificationSettings;

public record NotificationSettingsDto(
	bool IsTelegramNotificationsEnabled,
	bool NewPayoutNotification,
	bool NewElectricityPaymentNotification,
	bool NewPurchaseNotification,
	bool UnpaidElectricityPaymentReminder,
	bool UnpaidPurchasePaymentReminder);