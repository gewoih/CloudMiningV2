namespace Modules.Notifications.Contracts.DTO;

public record NotificationSettingsDto(
	bool IsTelegramNotificationsEnabled,
	bool NewPayoutNotification,
	bool NewElectricityPaymentNotification,
	bool NewPurchaseNotification,
	bool UnpaidElectricityPaymentReminder,
	bool UnpaidPurchasePaymentReminder);