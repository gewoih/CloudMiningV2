using CloudMining.Domain.Enums;

namespace CloudMining.Interfaces.DTO.Payments.User;

public class UserPaymentDto : PaymentDto
{
	public decimal Share { get; set; }
	public ShareStatus Status { get; set; }
	public decimal SharedAmount { get; set; }
}