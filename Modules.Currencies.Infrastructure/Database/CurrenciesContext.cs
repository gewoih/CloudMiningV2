using CloudMining.Common.Database;
using Microsoft.EntityFrameworkCore;
using Modules.Currencies.Domain.Models;

namespace Modules.Currencies.Infrastructure.Database;

public sealed class CurrenciesContext : CloudMiningContext
{
	public DbSet<Currency> Currencies { get; set; }
    
	public CurrenciesContext(DbContextOptions<CurrenciesContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Currency>().HasData(DatabaseInitializer.GetCurrencies());
		modelBuilder.Entity<Currency>().HasIndex(currency => currency.Code).IsUnique();
	}
}