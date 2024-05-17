using System.Security.Claims;
using CloudMining.Application.DTO.Users;
using CloudMining.Application.Services.JWT;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CloudMining.Application.Services.Users
{
	public sealed class UserService : IUserService
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly JwtService _jwtService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UserService(UserManager<User> userManager,
			SignInManager<User> signInManager,
			JwtService jwtService,
			IHttpContextAccessor httpContextAccessor)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_httpContextAccessor = httpContextAccessor;
			_jwtService = jwtService;
		}

		public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
		{
			//TODO: Добавить маппер
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

		public async Task<string> LoginAsync(LoginDto credentials)
		{
			var authResult =
				await _signInManager.PasswordSignInAsync(credentials.Email, credentials.Password, true, false);
			var jwt = string.Empty;

			if (authResult.Succeeded)
			{
				var user = await _userManager.FindByEmailAsync(credentials.Email);
				if (user is not null)
					jwt = await _jwtService.GenerateAsync(user);
			}

			return jwt;
		}

		public async Task<bool> ChangeEmailAsync(ChangeEmailDto dto)
		{
			var userId = GetCurrentUserId();
			if (userId == null)
				return false;

			var user = await _userManager.FindByIdAsync(userId.ToString());
			if (user is null)
				return false;

			var token = await _userManager.GenerateChangeEmailTokenAsync(user, dto.Email);
			var result = await _userManager.ChangeEmailAsync(user, dto.Email, token);

			user.UserName = user.Email;
			await _userManager.UpdateAsync(user);

			return result.Succeeded;
		}

		public Guid? GetCurrentUserId()
		{
			var authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
			if (string.IsNullOrEmpty(authHeader))
				return null;

			var jwt = authHeader.Split(' ')[1];
			var subClaim = _jwtService.GetSubClaim(jwt);
			return Guid.Parse(subClaim);
		}

		public bool IsCurrentUserAdmin()
		{
			var currentUserRoles = GetCurrentUserRoles();
			return currentUserRoles.Contains(UserRole.Admin);
		}

		private List<UserRole> GetCurrentUserRoles()
		{
			var roleClaims = _httpContextAccessor.HttpContext.User.Claims
				.Where(c => c.Type == ClaimTypes.Role)
				.Select(c => Enum.Parse<UserRole>(c.Value))
				.ToList();

			return roleClaims;
		}
	}
}