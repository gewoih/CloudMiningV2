using CloudMining.Domain.Models.Base;

namespace CloudMining.Domain.Models.Purchases;

public class Purchase : Entity
{
	public decimal Amount { get; set; }
	public DateTime Date { get; set; }
}