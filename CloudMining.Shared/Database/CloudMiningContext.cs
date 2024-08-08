using CloudMining.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Common.Database;

public abstract class CloudMiningContext : DbContext
{
	protected CloudMiningContext(DbContextOptions options) : base(options) 
	{
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

	public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
		CancellationToken cancellationToken = new())
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