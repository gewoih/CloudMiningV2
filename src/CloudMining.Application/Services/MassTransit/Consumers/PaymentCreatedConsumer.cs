using CloudMining.Application.Services.MassTransit.Events;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Notifications;
using CloudMining.Interfaces.Interfaces;
using MassTransit;

namespace CloudMining.Application.Services.MassTransit.Consumers;

public sealed class PaymentCreatedConsumer : IConsumer<PaymentCreated>
{
	private readonly IEnumerable<INotificationService> _notificationServices;

	public PaymentCreatedConsumer(
		IEnumerable<INotificationService> notificationServices)
	{
		_notificationServices = notificationServices;
	}

	public async Task Consume(ConsumeContext<PaymentCreated> context)
	{
		var message = string.Empty;
		var paymentType = context.Message.Payment.Type;

		foreach (var paymentShare in context.Message.Payment.PaymentShares)
		{
			message = paymentType switch
			{
				PaymentType.Electricity => "От Вас требуется оплата электричества.",
				PaymentType.Crypto => "Для Вас получена новая выплата!",
				_ => message
			};

			var shareInfo = Environment.NewLine;
			shareInfo += $"Общая сумма: {context.Message.Payment.Amount}";
			shareInfo += $"{Environment.NewLine}";
			shareInfo += $"Ваш процент: {paymentShare.Share}%";
			shareInfo += $"{Environment.NewLine}";
			shareInfo += $"Ваша доля: {paymentShare.Amount}";
			message += shareInfo;

			var notification = new Notification
			{
				UserId = paymentShare.UserId,
				Message = message
			};

			foreach (var notificationService in _notificationServices)
				await notificationService.SendAsync(notification);
		}
	}
}