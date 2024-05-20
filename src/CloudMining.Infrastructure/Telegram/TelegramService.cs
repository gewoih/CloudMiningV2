using CloudMining.Contracts.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CloudMining.Infrastructure.Telegram;

public class TelegramService : BackgroundService
{
	private readonly ITelegramBotClient _botClient;
	private readonly IServiceProvider _serviceProvider;

	public TelegramService(ITelegramBotClient botClient, IServiceProvider serviceProvider)
	{
		_botClient = botClient;
		_serviceProvider = serviceProvider;
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_botClient.StartReceiving(HandleUpdateAsync, HandlePollingErrorAsync, cancellationToken: stoppingToken);
		return Task.CompletedTask;
	}
	
	private async Task HandleUpdateAsync(
		ITelegramBotClient botClient,
		Update update,
		CancellationToken cancellationToken)
	{
		if (update.Type is UpdateType.Message)
		{
			var chatId = update.Message.Chat.Id;
			
			await using var scope = _serviceProvider.CreateAsyncScope();
			var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

			var isUpdated = await userService.ChangeTelegramChatIdAsync(update.Message.Chat.Username, chatId);
			var message = isUpdated 
				? "Данные успешно обновлены!" 
				: "Произошла ошибка. \nПожалуйста, укажите ваш TelegramUsername на сайте CloudMining.";
			
			await botClient.SendTextMessageAsync(
				chatId: chatId,
				text: message,
				cancellationToken: cancellationToken);
		}
	}

	private static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}