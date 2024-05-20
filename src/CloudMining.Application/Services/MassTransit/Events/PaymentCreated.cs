using CloudMining.Domain.Models.Payments.Shareable;

namespace CloudMining.Application.Services.MassTransit.Events;

public sealed class PaymentCreated
{
	public ShareablePayment Payment { get; set; }
}