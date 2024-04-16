using Newtonsoft.Json;

namespace CloudMining.Infrastructure.Emcd.Models
{
	public sealed class Payout
	{
		public long Timestamp { get; set; }

		[JsonProperty("gmt_time")]
		public DateTime GmtTime { get; set; }

		public decimal Amount { get; set; }
		public string TxId { get; set; }
	}
}
