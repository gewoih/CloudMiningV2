using CloudMining.Domain.Models.Base;

namespace CloudMining.Domain.Models
{
	public class Currency : Entity
	{
        public CurrencyCode Code { get; set; }
        public int Precision { get; set; }
	}
}
