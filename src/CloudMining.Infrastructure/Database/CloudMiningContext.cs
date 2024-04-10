using CloudMining.Domain.Models;
using CloudMining.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Infrastructure.Database
{
	public class CloudMiningContext : IdentityDbContext<User, Role, Guid>
	{
		public DbSet<Currency> Currencies { get; set; }
		public DbSet<Deposit> Deposits { get; set; }
		public DbSet<PaymentShare> PaymentShares { get; set; }
		public DbSet<ShareablePayment> ShareablePayments { get; set; }

		public CloudMiningContext(DbContextOptions<CloudMiningContext> options) : base(options) { }
	}
}
