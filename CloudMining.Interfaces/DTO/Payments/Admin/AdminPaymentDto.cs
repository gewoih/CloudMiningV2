namespace CloudMining.Interfaces.DTO.Payments.Admin;

public class AdminPaymentDto : PaymentDto
{
    public bool IsCompleted { get; set; }
    public string? Caption { get; set; }
}