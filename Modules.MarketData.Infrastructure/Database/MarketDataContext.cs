using CloudMining.Common.Database;
using Microsoft.EntityFrameworkCore;

namespace Modules.MarketData.Infrastructure.Database;

public sealed class MarketDataContext : CloudMiningContext
{
	public DbSet<Domain.Models.MarketData> MarketData { get; set; }
    
	public MarketDataContext(DbContextOptions<MarketDataContext> options) : base(options)
	{
	}
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Domain.Models.MarketData>()
			.HasIndex(data => new { data.From, data.To, data.Date })
			.IsUnique();
	}
}