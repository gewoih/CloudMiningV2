using CloudMining.Domain.Enums;
using CloudMining.Interfaces.DTO.Users;

namespace CloudMining.Interfaces.DTO.Payments.Admin;

public record PaymentShareDto(Guid Id, UserDto User, decimal Amount, decimal Share, ShareStatus Status);