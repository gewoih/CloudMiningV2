namespace CloudMining.Contracts.DTO.Payments.User;

public class UserPaymentDto : PaymentDto
{
    public decimal Share { get; set; }
    public decimal SharedAmount { get; set; }
}