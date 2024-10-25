namespace CloudMining.Api.Startup;

public static class CorsSetup
{
	public static IServiceCollection ConfigureCors(this IServiceCollection services)
	{
		services.AddCors(options =>
		{
			options.AddPolicy("AllowSpecificOrigin", configure =>
				configure.AllowAnyHeader()
					.AllowAnyMethod()
					.AllowAnyOrigin());
		});

		return services;
	}
}