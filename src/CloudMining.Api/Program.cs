using System.Text;
using CloudMining.Api.Startup;
using CloudMining.Common.Database;
using Microsoft.EntityFrameworkCore;
using Modules.Currencies.Infrastructure.Database;
using Modules.MarketData.Infrastructure.Database;
using Modules.Notifications.Infrastructure.Database;
using Modules.Payments.Infrastructure.Database;
using Modules.Users.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .ConfigureControllers()
    .ConfigureFluentValidation()
    .ConfigureCors()
    .ConfigureSwagger()
    .ConfigureSettings(builder.Configuration)
    .ConfigureDbContext(builder.Configuration)
    .ConfigureIdentity()
    .ConfigureAuthentication(builder.Configuration)
    .RegisterServices(builder.Configuration)
    .ConfigureMassTransit();

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var app = builder.Build();

var scope = app.Services.CreateScope();
await scope.ServiceProvider.GetService<UsersContext>()!.Database.MigrateAsync();
await scope.ServiceProvider.GetService<CurrenciesContext>()!.Database.MigrateAsync();
await scope.ServiceProvider.GetService<PaymentsContext>()!.Database.MigrateAsync();
await scope.ServiceProvider.GetService<NotificationsContext>()!.Database.MigrateAsync();
await scope.ServiceProvider.GetService<MarketDataContext>()!.Database.MigrateAsync();

if (app.Environment.IsDevelopment())
{
    await DatabaseInitializer.CreateRolesAsync(scope.ServiceProvider);
    await DatabaseInitializer.CreateUsersAsync(scope.ServiceProvider);

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();