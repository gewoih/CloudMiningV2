namespace CloudMining.Interfaces.DTO.Purchases;

public record CreatePurchaseDto(string? Caption, decimal Amount, DateTime Date);