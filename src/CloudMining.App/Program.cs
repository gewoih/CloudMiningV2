using CloudMining.App.Middleware;
using CloudMining.Application.Services.Currencies;
using CloudMining.Application.Services.Deposits;
using CloudMining.Application.Services.Payments;
using CloudMining.Application.Services.Payouts;
using CloudMining.Application.Services.Shares;
using CloudMining.Application.Services.Users;
using CloudMining.Domain.Models.Identity;
using CloudMining.Infrastructure.Database;
using CloudMining.Infrastructure.Emcd;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CloudMiningContext>(options =>
	options.UseNpgsql(connectionString));

builder.Services.AddIdentity<User, Role>(options =>
	{
		options.User.RequireUniqueEmail = true;
		options.SignIn.RequireConfirmedAccount = true;
	})
	.AddEntityFrameworkStores<CloudMiningContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/user/login";
	options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IShareService, ShareService>();
builder.Services.AddScoped<IShareablePaymentService, ShareablePaymentService>();
builder.Services.AddScoped<IDepositService, DepositService>();

builder.Services.AddHostedService<PayoutsLoaderService>();

builder.Services.AddScoped<AuthenticationMiddleware>();

builder.Services.AddHttpClient<EmcdApiClient>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

if (!app.Environment.IsDevelopment())
	app.UseMiddleware<AuthenticationMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

var scope = app.Services.CreateScope();
var database = scope.ServiceProvider.GetService<CloudMiningContext>()?.Database;
await database.MigrateAsync();

app.Run();
