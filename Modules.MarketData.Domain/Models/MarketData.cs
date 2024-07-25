using CloudMining.Domain.Models;
using Modules.Currencies.Domain.Enums;

namespace Modules.MarketData.Domain.Models;

public class MarketData : Entity
{
    public DateTime Date { get; set; }
    public CurrencyCode From { get; set; }
    public CurrencyCode To { get; set; }
    public decimal Price { get; set; }
}