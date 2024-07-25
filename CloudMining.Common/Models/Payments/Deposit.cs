using CloudMining.Common.Models.Identity;
using CloudMining.Common.Models.Shares;

namespace CloudMining.Common.Models.Payments;

public class Deposit : Payment
{
	public User User { get; set; }
	public Guid UserId { get; set; }
	public DateTime Date { get; set; }
	public List<ShareChange> ShareChanges { get; set; }
}