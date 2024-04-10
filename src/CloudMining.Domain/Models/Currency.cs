using CloudMining.Domain.Models.Base;

namespace CloudMining.Domain.Models
{
	public class Currency : Entity
	{
		public string Code { get; set; }
		public int Precision { get; set; }
	}
}
