using CloudMining.Domain.Enums;
using CloudMining.Domain.Models;
using CloudMining.Domain.Models.Currencies;
using CloudMining.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CloudMining.Infrastructure.Database
{
	public static class DatabaseInitializer
	{
		private static UserManager<User> _userManager;
		
		public static List<Currency> GetCurrencies()
		{
			return
			[
				new() { Id = Guid.NewGuid(), Caption = "Рубль", Code = CurrencyCode.RUB, Precision = 2 },
				new() { Id = Guid.NewGuid(), Caption = "Доллар", Code = CurrencyCode.USD, Precision = 2 },
				new() { Id = Guid.NewGuid(), Caption = "Bitcoin", Code = CurrencyCode.BTC, Precision = 4 },
				new() { Id = Guid.NewGuid(), Caption = "Etherium", Code = CurrencyCode.ETH, Precision = 4 },
				new() { Id = Guid.NewGuid(), Caption = "Litecoin", Code = CurrencyCode.LTC, Precision = 2 },
				new() { Id = Guid.NewGuid(), Caption = "Dogecoin", Code = CurrencyCode.DOGE, Precision = 0 }
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

			await RegisterUserIfNotExists("Admin", "nranenko@bk.ru", "Никита", "Раненко", "Максимович", "24042001Nr.");
			await RegisterUserIfNotExists("User", "grigmaks2014@yandex.ru", "Максим", "Григорьев", "Владимирович", "24042001Nr.");
		}

		private static async Task RegisterUserIfNotExists(string role, string email, string firstName, 
			string lastName, string patronymic, string password)
		{
			var user = new User
			{
				UserName = email, 
				Email = email, 
				FirstName = firstName, 
				LastName = lastName,
				Patronymic = patronymic
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
}
