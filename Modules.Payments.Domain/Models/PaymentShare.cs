using CloudMining.Domain.Models;
using Modules.Payments.Domain.Enums;

namespace Modules.Payments.Domain.Models;

public class PaymentShare : Entity
{
	public Guid UserId { get; set; }
	public Guid ShareablePaymentId { get; set; }
	public DateTime Date { get; set; }
	public decimal Amount { get; set; }
	public decimal Share { get; set; }
	public ShareStatus Status { get; set; }
}