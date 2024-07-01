using System.ComponentModel;

namespace CloudMining.Domain.Enums;

public enum CandlestickTimeFrame
{
    [Description("1m")]
    Minute,
    
    [Description("1h")]
    Hour,
    
    [Description("1d")]
    Day
}