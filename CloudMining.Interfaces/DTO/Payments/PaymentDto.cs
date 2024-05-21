using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Interfaces.DTO.Currencies;

namespace CloudMining.Interfaces.DTO.Payments;

public abstract class PaymentDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public CurrencyDto Currency { get; set; }
}