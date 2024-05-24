using CloudMining.Domain.Enums;
using CloudMining.Interfaces.DTO.Users;

namespace CloudMining.Interfaces.DTO.Payments.Admin;

public class PaymentShareDto
{
    public UserDto User { get; set; }
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public decimal Share { get; set; }
    public ShareStatus Status { get; set; }
}