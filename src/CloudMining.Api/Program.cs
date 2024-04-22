using CloudMining.Api.IdentityServer;
using CloudMining.Application.Services.Currencies;
using CloudMining.Application.Services.Deposits;
using CloudMining.Application.Services.Payments;
using CloudMining.Application.Services.Payouts;
using CloudMining.Application.Services.Shares;
using CloudMining.Application.Services.Users;
using CloudMining.Domain.Models.Identity;
using CloudMining.Infrastructure.Database;
using CloudMining.Infrastructure.Emcd;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.FullName;

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigin", configure =>
		configure.WithOrigins("http://localhost:8080")
			.AllowAnyMethod()
			.AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CloudMiningContext>(options =>
	options.UseNpgsql(connectionString));

builder.Services.AddIdentity<User, Role>(options =>
	{
		options.User.RequireUniqueEmail = true;
	})
	.AddEntityFrameworkStores<CloudMiningContext>()
	.AddDefaultTokenProviders();

builder.Services
	.AddIdentityServer(options =>
	{
		options.UserInteraction.LoginUrl = "/users/login";
	})
	.AddInMemoryApiScopes(IdentityServerConfig.GetApiScopes())
	.AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
	.AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
	.AddInMemoryClients(IdentityServerConfig.GetClients())
	.AddAspNetIdentity<User>()
	.AddDeveloperSigningCredential();

builder.Services.AddAuthentication();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IShareService, ShareService>();
builder.Services.AddScoped<IShareablePaymentService, ShareablePaymentService>();
builder.Services.AddScoped<IDepositService, DepositService>();

builder.Services.AddHttpClient<EmcdApiClient>();
builder.Services.AddHostedService<PayoutsLoaderService>();

var app = builder.Build();
var scope = app.Services.CreateScope();
var database = scope.ServiceProvider.GetService<CloudMiningContext>()?.Database;
await database.MigrateAsync();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

app.UseIdentityServer();
app.UseAuthorization();

app.MapControllers();

app.Run();
