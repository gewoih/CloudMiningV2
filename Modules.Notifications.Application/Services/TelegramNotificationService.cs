using Modules.Notifications.Contracts.Interfaces;
using Modules.Notifications.Domain.Models;
using Modules.Users.Contracts.Interfaces;
using Telegram.Bot;

namespace Modules.Notifications.Application.Services;

public sealed class TelegramNotificationService : INotificationService
{
	private readonly INotificationManagementService _notificationManagementService;
	private readonly INotificationSettingsService _notificationSettingsService;
	private readonly ITelegramBotClient _telegramBotClient;
	private readonly IUserManagementService _userManagementService;

	public TelegramNotificationService(IUserManagementService userManagementService,
		ITelegramBotClient telegramBotClient,
		INotificationSettingsService notificationSettingsService,
		INotificationManagementService notificationManagementService)
	{
		_userManagementService = userManagementService;
		_telegramBotClient = telegramBotClient;
		_notificationSettingsService = notificationSettingsService;
		_notificationManagementService = notificationManagementService;
	}

	public async Task<Notification?> SendAsync(Notification notification)
	{
		var user = await _userManagementService.GetAsync(notification.UserId);
		if (user is null)
			return null;

		var userNotificationSettings = await _notificationSettingsService.GetUserSettingsAsync(user.Id);

		var userChatId = user.TelegramChatId;
		if (!userChatId.HasValue || !userNotificationSettings.IsTelegramNotificationsEnabled)
			return null;

		await _telegramBotClient.SendTextMessageAsync(userChatId, notification.Message);

		notification.Method = NotificationMethod.Telegram;
		notification.UserIdentifier = user.TelegramUsername;
		var createdNotification = await _notificationManagementService.AddAsync(notification);

		return createdNotification;
	}
}