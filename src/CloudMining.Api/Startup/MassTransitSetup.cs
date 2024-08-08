using MassTransit;
using Modules.Notifications.Application.Services.MassTransit.Consumers;
using Modules.Notifications.Infrastructure.Database;

namespace CloudMining.Api.Startup;

public static class MassTransitSetup
{
	public static IServiceCollection ConfigureMassTransit(this IServiceCollection services)
	{
		services.AddMassTransit(x =>
		{
			x.AddEntityFrameworkOutbox<NotificationsContext>(configurator =>
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