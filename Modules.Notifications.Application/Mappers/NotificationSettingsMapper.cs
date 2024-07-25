using CloudMining.Common.Mappers;
using CloudMining.Common.Models.UserSettings;
using Modules.Notifications.Contracts.DTO;

namespace Modules.Notifications.Application.Mappers;

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