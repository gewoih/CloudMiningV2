using CloudMining.Domain.Models;
using CloudMining.Domain.Models.Base;
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
		public DbSet<ShareChange> ShareChanges { get; set; }

		public CloudMiningContext(DbContextOptions<CloudMiningContext> options) : base(options) { }

		public override int SaveChanges()
		{
			OnBeforeSaving();
			return base.SaveChanges();
		}

		public override int SaveChanges(bool acceptAllChangesOnSuccess)
		{
			OnBeforeSaving();
			return base.SaveChanges(acceptAllChangesOnSuccess);
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
		{
			OnBeforeSaving();
			return base.SaveChangesAsync(cancellationToken);
		}

		public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new())
		{
			OnBeforeSaving();
			return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
		}

		private void OnBeforeSaving()
		{
			var entries = ChangeTracker.Entries();
			foreach (var entry in entries)
			{
				var utcNow = DateTime.UtcNow;
				if (entry.Entity is Entity trackable)
				{
					switch (entry.State)
					{
						case EntityState.Modified:
							trackable.UpdatedDate = utcNow;
							break;
						case EntityState.Added:
							trackable.CreatedDate = utcNow;
							trackable.UpdatedDate = utcNow;
							break;
						case EntityState.Deleted:
							entry.State = EntityState.Modified;
							trackable.IsDeleted = true;
							trackable.DeletedDate = utcNow;
							break;
					}
				}
			}
		}
	}
}
