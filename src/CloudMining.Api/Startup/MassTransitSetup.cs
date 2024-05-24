using CloudMining.Application.Services.MassTransit.Consumers;
using CloudMining.Infrastructure.Database;
using MassTransit;

namespace CloudMining.Api.Startup;

public static class MassTransitSetup
{
	public static IServiceCollection ConfigureMassTransit(this IServiceCollection services)
	{
		services.AddMassTransit(x =>
		{
			x.AddEntityFrameworkOutbox<CloudMiningContext>(configurator =>
			{
				configurator.UsePostgres();
				configurator.UseBusOutbox();
				configurator.QueryDelay = TimeSpan.FromSeconds(10);
			});
			
			x.AddConsumer<PaymentCreatedConsumer>();
			
			x.UsingInMemory((context, cfg) =>
			{
				cfg.AutoStart = true;
				cfg.ConfigureEndpoints(context);
			});
		});

		return services;
	}
}