using CloudMining.Interfaces.DTO.Currencies;

namespace CloudMining.Interfaces.DTO.Payments;

public abstract class PaymentDto
{
    public Guid Id { get; init; }
    public DateTime Date { get; init; }
    public decimal Amount { get; init; }
    public CurrencyDto Currency { get; init; }
    public string? Caption { get; init; }
}