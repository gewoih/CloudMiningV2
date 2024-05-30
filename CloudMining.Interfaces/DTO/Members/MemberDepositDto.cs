namespace CloudMining.Interfaces.DTO.Members;

public record MemberDepositDto(Guid Id, DateTime Date, decimal Amount, string? Caption);