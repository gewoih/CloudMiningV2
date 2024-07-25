using System.Text;
using CloudMining.Api.Startup;
using CloudMining.Common.Database;
using Microsoft.EntityFrameworkCore;

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
var database = scope.ServiceProvider.GetService<CloudMiningContext>()?.Database;
await database.MigrateAsync();

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