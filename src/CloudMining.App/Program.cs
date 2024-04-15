﻿using CloudMining.App.Middleware;
using CloudMining.Application.Services.Currencies;
using CloudMining.Application.Services.Deposits;
using CloudMining.Application.Services.Payments;
using CloudMining.Application.Services.Shares;
using CloudMining.Application.Services.Users;
using CloudMining.Domain.Models.Identity;
using CloudMining.Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CloudMiningContext>(options =>
	options.UseNpgsql(connectionString));

builder.Services.AddIdentity<User, Role>()
	.AddEntityFrameworkStores<CloudMiningContext>()
	.AddDefaultTokenProviders();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IShareService, ShareService>();
builder.Services.AddScoped<IShareablePaymentService, ShareablePaymentService>();
builder.Services.AddScoped<IDepositService, DepositService>();

builder.Services.AddScoped<AuthenticationMiddleware>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AuthenticationMiddleware>();

app.MapDefaultControllerRoute();

var scope = app.Services.CreateScope();
var database = scope.ServiceProvider.GetService<CloudMiningContext>()?.Database;
await database.MigrateAsync();

app.Run();
