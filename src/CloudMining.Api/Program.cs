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
using System.Text;
using AutoMapper;
using CloudMining.Application.Mappings;
using CloudMining.Application.Services.JWT;
using CloudMining.Domain.Models.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
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

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"])),
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateLifetime = true
	};
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);
var config = new MapperConfiguration(cfg => {
	cfg.AddProfile<GlobalMappingProfile>();
});


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IShareService, ShareService>();
builder.Services.AddScoped<IShareablePaymentService, ShareablePaymentService>();
builder.Services.AddScoped<IDepositService, DepositService>();
builder.Services.AddSingleton<JwtService>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
