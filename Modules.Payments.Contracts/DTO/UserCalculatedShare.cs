namespace Modules.Payments.Contracts.DTO;

public record UserCalculatedShare(Guid UserId, decimal Amount, decimal Share);