using CloudMining.Domain.Models.Notifications;
using CloudMining.Infrastructure.Database;
using CloudMining.Interfaces.Interfaces;
using Telegram.Bot;

namespace CloudMining.Application.Services;

public sealed class TelegramNotificationService : INotificationService
{
	private readonly ITelegramBotClient _telegramBotClient;
	private readonly IUserService _userService;
	private readonly CloudMiningContext _context;

	public TelegramNotificationService(IUserService userService, ITelegramBotClient telegramBotClient, CloudMiningContext context)
	{
		_userService = userService;
		_telegramBotClient = telegramBotClient;
		_context = context;
	}

	public async Task<Notification> SendAsync(Notification notification)
	{
		var users = await _userService.GetUsersWithNotificationSettingsAsync();
		var usersWithTelegramNotifications =
			users.Where(user => user is
				{ NotificationSettings.IsTelegramNotificationsEnabled: true, TelegramChatId: not null });

		foreach (var user in usersWithTelegramNotifications)
		{
			var userChatId = user.TelegramChatId;
			await _telegramBotClient.SendTextMessageAsync(userChatId, notification.Message);

			notification.Method = NotificationMethod.Telegram;
			notification.UserIdentifier = user.TelegramUsername;
			notification.UserId = user.Id;
			
			await _context.Notifications.AddAsync(notification);
			await _context.SaveChangesAsync();
		}

		return notification;
	}
}