using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CloudMining.Infrastructure.Database;

public static class DatabaseInitializer
{
	private static UserManager<User> _userManager;

	public static List<Currency> GetCurrencies()
	{
		return
		[
			new Currency
			{
				Id = new Guid("E23A58C6-9CEF-4C6F-94FC-8577A4B4FB84"), Caption = "Рубль", Code = CurrencyCode.RUB,
				ShortName = "₽", Precision = 2
			},
			new Currency
			{
				Id = new Guid("F1DEBADF-A2C3-4908-A11C-8329DF252FB8"), Caption = "Доллар", Code = CurrencyCode.USD,
				ShortName = "$", Precision = 2
			},
			new Currency
			{
				Id = new Guid("9A3E4016-240E-44E3-A002-0EE2CFF499A3"), Caption = "Bitcoin", Code = CurrencyCode.BTC,
				ShortName = "BTC", Precision = 5
			},
			new Currency
			{
				Id = new Guid("8927702D-AE2E-4A5A-AF19-2E6FA1824648"), Caption = "Etherium", Code = CurrencyCode.ETH,
				ShortName = "ETH", Precision = 4
			},
			new Currency
			{
				Id = new Guid("62C520F6-499A-499E-86C2-B2AF9C8EBA42"), Caption = "Litecoin", Code = CurrencyCode.LTC,
				ShortName = "LTC", Precision = 2
			},
			new Currency
			{
				Id = new Guid("C24B466A-97C2-4D64-BBE7-C583B76A2C3C"), Caption = "Tether", Code = CurrencyCode.USDT,
				ShortName = "USDT", Precision = 2
			},
			new Currency
			{
				Id = new Guid("A5450179-3BFF-4645-9209-04ACC6168C5B"), Caption = "Dogecoin", Code = CurrencyCode.DOGE,
				ShortName = "DOGE", Precision = 0
			}
		];
	}

	public static async Task CreateRolesAsync(IServiceProvider serviceProvider)
	{
		var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

		foreach (var roleName in Enum.GetNames(typeof(UserRole)))
		{
			var roleExist = await roleManager.RoleExistsAsync(roleName);
			if (!roleExist)
				await roleManager.CreateAsync(new Role { Name = roleName });
		}
	}

	public static async Task CreateUsersAsync(IServiceProvider serviceProvider)
	{
		_userManager = serviceProvider.GetRequiredService<UserManager<User>>();

		await RegisterUserIfNotExists("Admin", 
			"nranenko@bk.ru", 
			"Никита", 
			"Раненко", 
			"Максимович", 
			"24042001Nr.", 
			new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc));
		
		await RegisterUserIfNotExists("User", 
			"grigmaks2014@yandex.ru", 
			"Максим", 
			"Григорьев", 
			"Владимирович",
			"24042001Nr.", 
			new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc));
	}

	private static async Task RegisterUserIfNotExists(string role, string email, string firstName,
		string lastName, string patronymic, string password, DateTime registrationDate)
	{
		var user = new User
		{
			UserName = email,
			Email = email,
			FirstName = firstName,
			LastName = lastName,
			Patronymic = patronymic,
			RegistrationDate = registrationDate
		};

		var userExist = await _userManager.FindByEmailAsync(email);
		if (userExist == null)
		{
			var createUser = await _userManager.CreateAsync(user, password);
			if (createUser.Succeeded)
				await _userManager.AddToRoleAsync(user, role);
		}
	}
}