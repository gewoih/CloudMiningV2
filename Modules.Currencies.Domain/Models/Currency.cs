using CloudMining.Domain.Models;
using Modules.Currencies.Domain.Enums;

namespace Modules.Currencies.Domain.Models;

public class Currency : Entity
{
	public CurrencyCode Code { get; set; }
	public string ShortName { get; set; }
	public int Precision { get; set; }
}