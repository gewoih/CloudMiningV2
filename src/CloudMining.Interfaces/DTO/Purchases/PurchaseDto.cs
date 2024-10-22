namespace CloudMining.Interfaces.DTO.Purchases;

public record PurchaseDto(Guid Id, string? Caption, decimal Amount, DateOnly Date);