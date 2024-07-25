using CloudMining.Common.Models.Payments.Shareable;

namespace Modules.Notifications.Application.Services.MassTransit.Events;

public sealed class PaymentCreated
{
	public ShareablePayment Payment { get; set; }
}