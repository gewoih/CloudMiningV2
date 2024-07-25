using Modules.Payments.Domain.Enums;

namespace Modules.Payments.Contracts.DTO.User;

public class UserPaymentDto : PaymentDto
{
	public decimal Share { get; set; }
	public ShareStatus Status { get; set; }
	public decimal SharedAmount { get; set; }
}