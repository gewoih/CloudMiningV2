using CloudMining.Contracts.Interfaces;
using CloudMining.Domain.Models.Notifications;
using Telegram.Bot;

namespace CloudMining.Application.Services;

public sealed class TelegramNotificationService : INotificationService
{
	private readonly ITelegramBotClient _telegramBotClient;
	private readonly IUserService _userService;

	public TelegramNotificationService(IUserService userService, ITelegramBotClient telegramBotClient)
	{
		_userService = userService;
		_telegramBotClient = telegramBotClient;
	}

	public async Task<Notification> SendAsync(Notification notification)
	{
		var users = await _userService.GetUsersWithNotificationSettingsAsync();
		var usersWithTelegramNotifications =
			users.Where(user => user is
				{ NotificationSettings: { IsTelegramNotificationsEnabled: true }, TelegramChatId: not null });

		foreach (var user in usersWithTelegramNotifications)
		{
			var userChatId = user.TelegramChatId;
			await _telegramBotClient.SendTextMessageAsync(userChatId, "Привет, у тебя новый платеж");
		}

		return notification;
	}
}