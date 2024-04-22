using CloudMining.Application.DTO.Users;
using CloudMining.Domain.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CloudMining.Application.Services.Users
{
	public sealed class UserService : IUserService
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UserService(UserManager<User> userManager, SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
		{
			var newUser = new User
			{
				Email = dto.Email,
				UserName = dto.Email,
				FirstName = dto.FirstName,
				LastName = dto.LastName,
				Patronymic = dto.Patronymic
			};

			return await _userManager.CreateAsync(newUser, dto.Password);
		}

		public async Task<SignInResult> LoginAsync(LoginDto credentials)
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

        public Guid? GetCurrentUserId()
        {
			var httpContext = _httpContextAccessor.HttpContext;
			var userId = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			return string.IsNullOrEmpty(userId) ? null : Guid.Parse(userId);
        }
	}
}
