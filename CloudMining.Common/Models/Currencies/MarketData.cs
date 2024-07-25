using CloudMining.Common.Models.Base;
using Modules.Currencies.Domain.Enums;

namespace CloudMining.Common.Models.Currencies;

public class MarketData : Entity
{
    public DateTime Date { get; set; }
    public CurrencyCode From { get; set; }
    public CurrencyCode To { get; set; }
    public decimal Price { get; set; }
}