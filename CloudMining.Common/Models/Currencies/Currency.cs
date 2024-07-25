using CloudMining.Common.Models.Base;
using Modules.Currencies.Domain.Enums;

namespace CloudMining.Common.Models.Currencies;

public class Currency : Entity
{
	public CurrencyCode Code { get; set; }
	public string ShortName { get; set; }
	public int Precision { get; set; }
}