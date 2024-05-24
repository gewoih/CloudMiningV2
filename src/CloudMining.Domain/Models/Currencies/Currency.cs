using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Base;

namespace CloudMining.Domain.Models.Currencies
{
	public class Currency : Entity
	{
        public CurrencyCode Code { get; set; }
        public string ShortName { get; set; }
        public int Precision { get; set; }
	}
}
