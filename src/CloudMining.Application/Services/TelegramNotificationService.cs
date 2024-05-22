using CloudMining.Domain.Models.Notifications;
using CloudMining.Infrastructure.Database;
using CloudMining.Interfaces.Interfaces;
using Telegram.Bot;

namespace CloudMining.Application.Services;

public sealed class TelegramNotificationService : INotificationService
{
	private readonly ITelegramBotClient _telegramBotClient;
	private readonly CloudMiningContext _context;
	private readonly IUserService _userService;
	private readonly INotificationSettingsService _notificationSettingsService;

	public TelegramNotificationService(IUserService userService, ITelegramBotClient telegramBotClient,
		CloudMiningContext context, INotificationSettingsService notificationSettingsService)
	{
		_userService = userService;
		_telegramBotClient = telegramBotClient;
		_context = context;
		_notificationSettingsService = notificationSettingsService;
	}

	public async Task<Notification?> SendAsync(Notification notification)
	{
		var user = await _userService.GetAsync(notification.UserId);
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