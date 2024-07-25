using CloudMining.Domain.Models;

namespace Modules.Payments.Domain.Models;

public sealed class ShareChange : Entity
{
	public Guid UserId { get; set; }
	public Guid DepositId { get; set; }
	public DateTime Date { get; set; }
	public decimal Before { get; set; }
	public decimal After { get; set; }
	public decimal Change => After - Before;
}