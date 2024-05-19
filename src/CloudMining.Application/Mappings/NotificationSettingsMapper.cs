using CloudMining.Application.DTO.NotificationSettings;
using CloudMining.Application.Services.Users;
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
			NewPayoutNotification = dto.NewPayoutNotification,
			NewPurchaseNotification = dto.NewPurchaseNotification,
			NewElectricityPaymentNotification = dto.NewElectricityPaymentNotification,
			UnpaidElectricityPaymentReminder = dto.UnpaidElectricityPaymentReminder,
			UnpaidPurchasePaymentReminder = dto.UnpaidPurchasePaymentReminder
		};
	}
}