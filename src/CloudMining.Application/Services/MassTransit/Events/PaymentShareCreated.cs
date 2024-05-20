using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Payments.Shareable;

namespace CloudMining.Application.Services.MassTransit.Events;

public sealed class PaymentShareCreated
{
	public PaymentType PaymentType { get; set; }
	public PaymentShare PaymentShare { get; set; }
}