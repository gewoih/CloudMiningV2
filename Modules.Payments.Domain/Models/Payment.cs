using CloudMining.Domain.Models;
using Modules.Currencies.Domain.Models;

namespace Modules.Payments.Domain.Models;

public abstract class Payment : Entity
{
	public decimal Amount { get; set; }
	public Guid CurrencyId { get; set; }
	public Currency Currency { get; set; }
	public DateTime Date { get; set; }
}