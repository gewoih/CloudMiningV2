﻿using System.Reflection;
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
using CloudMining.Api.Filters;
using CloudMining.Api.Validators.Deposit;
using CloudMining.Api.Validators.Payment;
using CloudMining.Api.Validators.User;
using CloudMining.Application.DTO.Payments;
using CloudMining.Application.DTO.Payments.Deposits;
using CloudMining.Application.DTO.Users;
using CloudMining.Application.Mappings;
using CloudMining.Application.Services.JWT;
using CloudMining.Domain.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using CloudMining.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
	options.Filters.Add<GlobalExceptionFilter>();
})
.AddNewtonsoftJson(options =>
{
	options.SerializerSettings.Converters.Add(new StringEnumConverter());
});

builder.Services.AddProblemDetails();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<LoginDto>, LoginValidator>();
builder.Services.AddScoped<IValidator<RegisterDto>, RegisterValidator>();
builder.Services.AddScoped<IValidator<CreatePaymentDto>, PaymentValidator>();
builder.Services.AddScoped<IValidator<CreateDepositDto>, DepositValidator>();

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigin", configure =>
		configure.WithOrigins("http://localhost:8080")
			.AllowAnyMethod()
			.AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
	opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
	opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = "Please enter token",
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		BearerFormat = "JWT",
		Scheme = "bearer"
	});

	opt.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type=ReferenceType.SecurityScheme,
					Id="Bearer"
				}
			},
			new string[]{}
		}
	});
});

builder.Services.Configure<EmcdSettings>(builder.Configuration.GetSection(EmcdSettings.SectionName));
builder.Services.Configure<PayoutsLoaderSettings>(builder.Configuration.GetSection(PayoutsLoaderSettings.SectionName));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

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

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IShareService, ShareService>();
builder.Services.AddScoped<IShareablePaymentService, ShareablePaymentService>();
builder.Services.AddScoped<IDepositService, DepositService>();
builder.Services.AddSingleton<JwtService>();

builder.Services.AddSingleton<IMapper<ShareablePayment, PaymentDto>, PaymentMapper>();
builder.Services.AddSingleton<IMapper<PaymentShare, PaymentShareDto>, PaymentShareMapper>();
builder.Services.AddSingleton<IMapper<Deposit, CreateDepositDto>, DepositMapper>();

builder.Services.AddHttpClient<EmcdApiClient>();
//builder.Services.AddHostedService<PayoutsLoaderService>();

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
