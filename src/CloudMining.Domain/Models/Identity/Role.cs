using Microsoft.AspNetCore.Identity;

namespace CloudMining.Domain.Models.Identity;

public class Role : IdentityRole<Guid>
{
	public decimal CommissionPercent { get; set; }
}