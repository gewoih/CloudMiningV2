namespace Modules.Payments.Contracts.DTO.Admin;

public class AdminPaymentDto : PaymentDto
{
	public bool IsCompleted { get; set; }
}