namespace CloudMining.Application.DTO.Payments;

public class PaymentListDto
{
    public List<PaymentDto>  Payments { get; set; }
    public int Count { get; set; }
}