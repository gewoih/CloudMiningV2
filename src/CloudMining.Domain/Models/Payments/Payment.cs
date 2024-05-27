using CloudMining.Domain.Models.Base;
using CloudMining.Domain.Models.Currencies;

namespace CloudMining.Domain.Models.Payments;

public abstract class Payment : Entity
{
	public decimal Amount { get; set; }
	public Guid CurrencyId { get; set; }
	public Currency Currency { get; set; }
	public DateTime Date { get; set; }
}