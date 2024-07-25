using System.ComponentModel;

namespace Modules.MarketData.Domain.Enums;

public enum CandlestickTimeFrame
{
    [Description("1m")]
    Minute,
    
    [Description("1h")]
    Hour,
    
    [Description("1d")]
    Day
}