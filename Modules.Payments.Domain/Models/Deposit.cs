namespace Modules.Payments.Domain.Models;

public class Deposit : Payment
{
	public Guid UserId { get; set; }
	public DateTime Date { get; set; }
	public List<ShareChange> ShareChanges { get; set; }
}