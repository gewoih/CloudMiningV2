using Modules.Payments.Domain.Models;

namespace Modules.Notifications.Application.Services.MassTransit.Events;

public class ShareablePaymentCreated
{
	public ShareablePayment Payment { get; set; }
}