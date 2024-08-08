using CloudMining.Common.Database;
using Microsoft.EntityFrameworkCore;
using Modules.Currencies.Infrastructure.Database;
using Modules.MarketData.Infrastructure.Database;
using Modules.Notifications.Infrastructure.Database;
using Modules.Payments.Infrastructure.Database;
using Modules.Users.Infrastructure.Database;

namespace CloudMining.Api.Startup;

public static class DbContextSetup
{
	public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("DefaultConnection");
		services.AddDbContext<UsersContext>(options => options.UseNpgsql(connectionString));
		services.AddDbContext<CurrenciesContext>(options => options.UseNpgsql(connectionString));
		services.AddDbContext<PaymentsContext>(options => options.UseNpgsql(connectionString));
		services.AddDbContext<NotificationsContext>(options => options.UseNpgsql(connectionString));
		services.AddDbContext<MarketDataContext>(options => options.UseNpgsql(connectionString));

		return services;
	}
}