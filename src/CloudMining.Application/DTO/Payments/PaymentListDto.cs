namespace CloudMining.Application.DTO.Payments;

public class PaymentListDto
{
    public List<PaymentDto> Payments { get; set; }
    public int TotalCount { get; set; }
}