namespace CloudMining.Application.DTO.Payments;

public class PaymentListDto
{
    public IEnumerable<PaymentDto> Payments { get; set; }
    public int TotalRecords { get; set; }
}