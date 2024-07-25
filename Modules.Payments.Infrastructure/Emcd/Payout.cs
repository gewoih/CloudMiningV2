using Newtonsoft.Json;

namespace Modules.Payments.Infrastructure.Emcd;

public sealed class Payout
{
	public string CoinName { get; set; }
	public long Timestamp { get; set; }

	[JsonProperty("gmt_time")] public DateTime GmtTime { get; set; }

	public decimal Amount { get; set; }
	public string TxId { get; set; }
}