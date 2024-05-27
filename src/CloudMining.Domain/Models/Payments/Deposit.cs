using CloudMining.Domain.Models.Identity;
using CloudMining.Domain.Models.Shares;

namespace CloudMining.Domain.Models.Payments;

public class Deposit : Payment
{
	public User User { get; set; }
	public Guid UserId { get; set; }
	public DateTime Date { get; set; }
	public List<ShareChange> ShareChanges { get; set; }
}