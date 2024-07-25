using Modules.Payments.Domain.Enums;

namespace Modules.Payments.Domain.Models;

public class ShareablePayment : Payment
{
	public PaymentType Type { get; set; }
	public List<PaymentShare> PaymentShares { get; set; }
	public bool IsCompleted => PaymentShares.TrueForAll(paymentShare => paymentShare.Status == ShareStatus.Completed);
}