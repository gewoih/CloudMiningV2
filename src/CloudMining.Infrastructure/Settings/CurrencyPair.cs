using CloudMining.Domain.Enums;

namespace CloudMining.Infrastructure.Settings;

public sealed class CurrencyPair
{
	public CurrencyCode From { get; set; }
	public CurrencyCode To { get; set; }
}