using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Base;

namespace CloudMining.Domain.Models.Currencies;

public class MarketData : Entity
{
    public DateTime Date { get; set; }
    public CurrencyCode From { get; set; }
    public CurrencyCode To { get; set; }
    public decimal Price { get; set; }
}