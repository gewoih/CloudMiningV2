using CloudMining.Common.Models.Payments.Shareable;

namespace Modules.Notifications.Application.Services.MassTransit.Events;

public class ShareablePaymentCreated
{
	public ShareablePayment Payment { get; set; }
}