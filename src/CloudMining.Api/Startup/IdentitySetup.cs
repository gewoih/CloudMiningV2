using CloudMining.Common.Database;
using Microsoft.AspNetCore.Identity;
using Modules.Users.Domain.Models;

namespace CloudMining.Api.Startup;

public static class IdentitySetup
{
	public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
	{
		services.AddIdentity<User, Role>(options => { options.User.RequireUniqueEmail = true; })
			.AddEntityFrameworkStores<CloudMiningContext>()
			.AddDefaultTokenProviders();

		return services;
	}
}