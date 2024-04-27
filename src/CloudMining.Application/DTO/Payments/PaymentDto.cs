namespace CloudMining.Application.DTO.Payments;

public class PaymentDto
{
    public Guid Id { get; set; }
    public string Caption { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public bool IsCompleted { get; set; }
}