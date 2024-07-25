using Modules.Payments.Contracts.DTO.User;
using Modules.Payments.Domain.Enums;

namespace Modules.Payments.Contracts.DTO.Admin;

public record PaymentShareDto(Guid Id, decimal Amount, decimal Share, ShareStatus Status);