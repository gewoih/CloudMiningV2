﻿using CloudMining.Domain.Models.Base;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Identity;
using CloudMining.Domain.Models.Notifications;
using CloudMining.Domain.Models.Payments;
using CloudMining.Domain.Models.Payments.Shareable;
using CloudMining.Domain.Models.Shares;
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
		public DbSet<Notification> Notifications { get; set; }

		public CloudMiningContext(DbContextOptions<CloudMiningContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<Currency>().HasData(DatabaseInitializer.GetCurrencies());
			builder.Entity<Currency>().HasIndex(currency => currency.Code).IsUnique();

			base.OnModelCreating(builder);
		}

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
