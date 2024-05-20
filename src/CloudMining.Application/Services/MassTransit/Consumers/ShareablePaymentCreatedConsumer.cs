using CloudMining.Application.Services.MassTransit.Events;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Notifications;
using CloudMining.Interfaces.Interfaces;
using MassTransit;

namespace CloudMining.Application.Services.MassTransit.Consumers;

public sealed class ShareablePaymentCreatedConsumer : IConsumer<ShareablePaymentCreated>
{
	private readonly IEnumerable<INotificationService> _notificationServices;

	public ShareablePaymentCreatedConsumer(IEnumerable<INotificationService> notificationServices)
	{
		_notificationServices = notificationServices;
	}

	public async Task Consume(ConsumeContext<ShareablePaymentCreated> context)
	{
		var message = string.Empty;
		var paymentType = context.Message.Payment.Type;
		message = paymentType switch
		{
			PaymentType.Electricity => "У вас новый платеж по электричеству!",
			PaymentType.Purchase => "У вас новая покупка!",
			PaymentType.Crypto => "У вас новая выплата!",
			_ => message
		};

		var notification = new Notification
		{
			Content = message,
		};
		
		foreach (var notificationService in _notificationServices)
		{
			await notificationService.SendAsync(notification);
		}
	}
}