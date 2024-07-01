using CloudMining.Domain.Enums;

namespace CloudMining.Infrastructure.Binance;

public sealed class PriceData
{
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    
}