namespace CloudMining.Contracts.DTO.Payments;

public abstract class PaymentDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public bool IsCompleted { get; set; }
}