using CloudMining.Domain.Models.UserSettings;
using CloudMining.Interfaces.DTO.NotificationSettings;

namespace CloudMining.Application.Mappings;

public sealed class NotificationSettingsMapper : IMapper<NotificationSettings, NotificationSettingsDto>
{
	public NotificationSettingsDto ToDto(NotificationSettings model)
	{
		return new NotificationSettingsDto(
			IsTelegramNotificationsEnabled: model.IsTelegramNotificationsEnabled,
			NewPayoutNotification: model.NewPayoutNotification,
			NewPurchaseNotification: model.NewPurchaseNotification,
			NewElectricityPaymentNotification: model.NewElectricityPaymentNotification,
			UnpaidElectricityPaymentReminder: model.UnpaidElectricityPaymentReminder,
			UnpaidPurchasePaymentReminder: model.UnpaidPurchasePaymentReminder
		);
	}

	public NotificationSettings ToDomain(NotificationSettingsDto dto)
	{
		return new NotificationSettings
		{
			IsTelegramNotificationsEnabled = dto.IsTelegramNotificationsEnabled,
			NewPayoutNotification = dto.NewPayoutNotification,
			NewPurchaseNotification = dto.NewPurchaseNotification,
			NewElectricityPaymentNotification = dto.NewElectricityPaymentNotification,
			UnpaidElectricityPaymentReminder = dto.UnpaidElectricityPaymentReminder,
			UnpaidPurchasePaymentReminder = dto.UnpaidPurchasePaymentReminder
		};
	}
}