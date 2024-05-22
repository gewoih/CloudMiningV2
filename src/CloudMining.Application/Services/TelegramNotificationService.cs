using CloudMining.Domain.Models.Notifications;
using CloudMining.Infrastructure.Database;
using CloudMining.Interfaces.Interfaces;
using Telegram.Bot;

namespace CloudMining.Application.Services;

public sealed class TelegramNotificationService : INotificationService
{
	private readonly ITelegramBotClient _telegramBotClient;
	private readonly CloudMiningContext _context;
	private readonly IUserManagementService _userManagementService;
	private readonly INotificationSettingsService _notificationSettingsService;

	public TelegramNotificationService(IUserManagementService userManagementService, ITelegramBotClient telegramBotClient,
		CloudMiningContext context, INotificationSettingsService notificationSettingsService)
	{
		_userManagementService = userManagementService;
		_telegramBotClient = telegramBotClient;
		_context = context;
		_notificationSettingsService = notificationSettingsService;
	}

	public async Task<Notification?> SendAsync(Notification notification)
	{
		var user = await _userManagementService.GetAsync(notification.UserId);
		var userNotificationSettings = await _notificationSettingsService.GetUserSettingsAsync(user?.Id);

		var userChatId = user?.TelegramChatId;
		if (!userChatId.HasValue)
			return null;

		if (!userNotificationSettings.IsTelegramNotificationsEnabled)
			return null;
		
		await _telegramBotClient.SendTextMessageAsync(userChatId, notification.Message);

		notification.Method = NotificationMethod.Telegram;
		notification.UserIdentifier = user.TelegramUsername;

		await _context.Notifications.AddAsync(notification);
		await _context.SaveChangesAsync();

		return notification;
	}
}