using CloudMining.Application.Services.MassTransit.Events;
using CloudMining.Application.Services.Notifications;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Notifications;
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
		if (context.Message.Payment.Type == PaymentType.Electricity)
			message = "У вас новый платеж по электричеству!";
		
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