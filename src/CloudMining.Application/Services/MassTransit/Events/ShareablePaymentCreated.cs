using CloudMining.Domain.Models.Payments.Shareable;

namespace CloudMining.Application.Services.MassTransit.Events;

public class ShareablePaymentCreated
{
	public ShareablePayment Payment { get; set; }
}