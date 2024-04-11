using CloudMining.Application.Models.Users;
using CloudMining.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services.Users
{
	public sealed class UserService : IUserService
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;

		public UserService(UserManager<User> userManager, SignInManager<User> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public async Task<IdentityResult> RegisterAsync(RegisterCredentials credentials)
		{
			var newUser = new User
			{
				Email = credentials.Email,
				UserName = credentials.Email,
				FirstName = credentials.FirstName,
				LastName = credentials.LastName,
				Patronymic = credentials.Patronymic
			};

			return await _userManager.CreateAsync(newUser, credentials.Password);
		}

		public async Task<SignInResult> LoginAsync(LoginCredentials credentials)
		{
			var authResult = await _signInManager.PasswordSignInAsync(credentials.Email, credentials.Password, true, false);
			return authResult;
		}

        public async Task<List<Guid>> GetAllUsersIdsAsync()
        {
            var usersIds = await _userManager.Users
	            .Select(u => u.Id)
	            .ToListAsync()
	            .ConfigureAwait(false);

            return usersIds;
        }
	}
}
