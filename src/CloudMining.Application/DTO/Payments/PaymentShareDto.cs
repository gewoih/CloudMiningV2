using CloudMining.Application.DTO.Users;

namespace CloudMining.Application.DTO.Payments;

public class PaymentShareDto
{
    public UserDto User { get; set; }
    public decimal Amount { get; set; }
    public decimal Share { get; set; }
    public bool IsCompleted { get; set; }
}