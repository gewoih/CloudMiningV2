using CloudMining.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Api.Startup;

public static class DbContextSetup
{
	public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("DefaultConnection");
		services.AddDbContext<CloudMiningContext>(options =>
			options.UseNpgsql(connectionString));

		return services;
	}
}