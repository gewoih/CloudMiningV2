using CloudMining.Domain.Enums;

namespace CloudMining.Infrastructure.Settings;

public sealed class CurrencyPair
{
	public CurrencyCode From { get; set; }
	public CurrencyCode To { get; set; }
	
	public override bool Equals(object? obj)
	{
		if (obj is CurrencyPair other)
		{
			return From == other.From && To == other.To;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(From, To);
	}
}