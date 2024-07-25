using CloudMining.Common.Models.Base;
using CloudMining.Common.Models.Currencies;

namespace CloudMining.Common.Models.Payments;

public abstract class Payment : Entity
{
	public decimal Amount { get; set; }
	public Guid CurrencyId { get; set; }
	public Currency Currency { get; set; }
	public DateTime Date { get; set; }
}