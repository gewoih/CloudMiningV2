namespace CloudMining.Interfaces.DTO.Payments.Deposits;

public record CreateDepositDto(Guid UserId, decimal Amount, DateTime Date);