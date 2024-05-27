using CloudMining.Domain.Enums;

namespace CloudMining.Domain.Models.Payments.Shareable;

public class ShareablePayment : Payment
{
	public PaymentType Type { get; set; }
	public List<PaymentShare> PaymentShares { get; set; }
	public bool IsCompleted => PaymentShares.TrueForAll(paymentShare => paymentShare.Status == ShareStatus.Completed);
}