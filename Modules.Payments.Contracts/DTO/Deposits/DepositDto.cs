namespace Modules.Payments.Contracts.DTO.Deposits;

public record DepositDto(Guid UserId, decimal Amount, DateTime Date);