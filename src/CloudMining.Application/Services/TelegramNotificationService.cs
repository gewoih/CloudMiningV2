using CloudMining.Domain.Models.Notifications;
using CloudMining.Interfaces.Interfaces;
using Telegram.Bot;

namespace CloudMining.Application.Services;

public sealed class TelegramNotificationService : INotificationService
{
	private readonly ITelegramBotClient _telegramBotClient;
	private readonly IUserManagementService _userManagementService;
	private readonly INotificationSettingsService _notificationSettingsService;
	private readonly INotificationManagementService _notificationManagementService;

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