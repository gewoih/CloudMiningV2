using CloudMining.Api.Filters;
using Newtonsoft.Json.Converters;

namespace CloudMining.Api.Startup;

public static class ControllersSetup
{
	public static IServiceCollection ConfigureControllers(this IServiceCollection services)
	{
		services.AddControllers(options => { options.Filters.Add<GlobalExceptionFilter>(); })
			.AddNewtonsoftJson(options => { options.SerializerSettings.Converters.Add(new StringEnumConverter()); });

		services.AddProblemDetails();

		return services;
	}
}