using CloudMining.Contracts.DTO.NotificationSettings;
using CloudMining.Contracts.Interfaces;
using CloudMining.Domain.Models.UserSettings;

namespace CloudMining.Application.Mappings;

public sealed class NotificationSettingsMapper : IMapper<NotificationSettings, NotificationSettingsDto>
{
	private readonly IUserService _userService;

	public NotificationSettingsMapper(IUserService userService)
	{
		_userService = userService;
	}

	public NotificationSettingsDto ToDto(NotificationSettings model)
	{
		return new NotificationSettingsDto
		{
			IsTelegramNotificationsEnabled = model.IsTelegramNotificationsEnabled,
			NewPayoutNotification = model.NewPayoutNotification,
			NewPurchaseNotification = model.NewPurchaseNotification,
			NewElectricityPaymentNotification = model.NewElectricityPaymentNotification,
			UnpaidElectricityPaymentReminder = model.UnpaidElectricityPaymentReminder,
			UnpaidPurchasePaymentReminder = model.UnpaidPurchasePaymentReminder
		};
	}

	public NotificationSettings ToDomain(NotificationSettingsDto dto)
	{
		var currentUserId = _userService.GetCurrentUserId() ?? Guid.Empty;

		return new NotificationSettings
		{
			UserId = currentUserId,
			IsTelegramNotificationsEnabled = dto.IsTelegramNotificationsEnabled,
			NewPayoutNotification = dto.NewPayoutNotification,
			NewPurchaseNotification = dto.NewPurchaseNotification,
			NewElectricityPaymentNotification = dto.NewElectricityPaymentNotification,
			UnpaidElectricityPaymentReminder = dto.UnpaidElectricityPaymentReminder,
			UnpaidPurchasePaymentReminder = dto.UnpaidPurchasePaymentReminder
		};
	}
}