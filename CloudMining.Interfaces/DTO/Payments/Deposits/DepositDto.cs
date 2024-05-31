namespace CloudMining.Interfaces.DTO.Payments.Deposits;

public record DepositDto(Guid UserId, decimal Amount, DateTime Date);