namespace CloudMining.Application.DTO.Payments;

public class PaymentsPageDto
{
    public List<PaymentDto> Items { get; set; }
    public int TotalCount { get; set; }
}