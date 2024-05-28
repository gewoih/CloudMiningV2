namespace CloudMining.Api.Startup;

public static class CorsSetup
{
	public static IServiceCollection ConfigureCors(this IServiceCollection services)
	{
		services.AddCors(options =>
		{
			options.AddPolicy("AllowSpecificOrigin", configure =>
				configure.WithOrigins("http://localhost:8080")
					.AllowAnyMethod()
					.AllowAnyHeader());
		});

		return services;
	}
}