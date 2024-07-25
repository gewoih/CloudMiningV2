using CloudMining.Common.Models.Base;
using CloudMining.Common.Models.Identity;
using Modules.Payments.Domain.Enums;

namespace CloudMining.Common.Models.Payments.Shareable;

public class PaymentShare : Entity
{
	public User User { get; set; }
	public Guid UserId { get; set; }
	public Guid ShareablePaymentId { get; set; }
	public DateTime Date { get; set; }
	public decimal Amount { get; set; }
	public decimal Share { get; set; }
	public ShareStatus Status { get; set; }
}