using CloudMining.Application.Services.Users;
using CloudMining.Domain.Models.Notifications;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CloudMining.Application.Services.Notifications;

public sealed class TelegramNotificationService : INotificationService
{
	private readonly ITelegramBotClient _telegramBotClient;
	private readonly IUserService _userService;

	public TelegramNotificationService(ITelegramBotClient telegramBotClient, IUserService userService)
	{
		_telegramBotClient = telegramBotClient;
		_userService = userService;
	}

	public async Task<Notification> SendAsync(Notification notification)
	{
		var users = await _userService.GetUsersWithNotificationSettingsAsync();
		var usersWithTelegramNotifications =
			users.Where(user => user.NotificationSettings is { IsTelegramNotificationsEnabled: true });
		
		foreach (var user in usersWithTelegramNotifications)
		{
			await _telegramBotClient.SendTextMessageAsync(new ChatId("@gewoih"), "Привет, у тебя новый платеж");
		}

		return notification;
	}
}