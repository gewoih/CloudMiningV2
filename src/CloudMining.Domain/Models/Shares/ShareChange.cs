using CloudMining.Domain.Models.Base;

namespace CloudMining.Domain.Models.Shares;

public sealed class ShareChange : Entity
{
	public Guid UserId { get; set; }
	public Guid DepositId { get; set; }
	public DateTime Date { get; set; }
	public decimal Before { get; set; }
	public decimal After { get; set; }
	public decimal Change => After - Before;
}