using System.Security.Claims;
using CloudMining.Contracts.DTO.File;
using CloudMining.Contracts.DTO.Users;
using CloudMining.Contracts.Interfaces;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.Application.Services
{
	public sealed class UserService : IUserService
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly JwtService _jwtService;
		private readonly IStorageService _storageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UserService(UserManager<User> userManager,
			SignInManager<User> signInManager,
			JwtService jwtService,
			IHttpContextAccessor httpContextAccessor, 
			IStorageService storageService)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_httpContextAccessor = httpContextAccessor;
			_storageService = storageService;
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

		public async Task<bool> ChangePasswordAsync(ChangePasswordDto dto)
		{
			var userId = GetCurrentUserId();
			if (userId == null)
				return false;

			var user = await _userManager.FindByIdAsync(userId.ToString());
			var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
			return result.Succeeded;
		}

		public async Task<string> ChangeAvatarAsync(FileDto dto)
		{
			var currentUserId = GetCurrentUserId();
			if (currentUserId == null)
				return string.Empty;

			var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
			if (currentUser is null)
				return string.Empty;
			
			var savedFilePath = await _storageService.SaveFileAsync(dto);
			currentUser.AvatarPath = savedFilePath;

			var result = await _userManager.UpdateAsync(currentUser);
			return result.Succeeded ? savedFilePath : string.Empty;
		}

		public async Task<bool> ChangeTelegramChatIdAsync(string telegramUsername, long telegramChatId)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(user => 
				user.TelegramUsername == telegramUsername);

			if (user is null)
				return false;

			user.TelegramChatId = telegramChatId;
			var result = await _userManager.UpdateAsync(user);
			return result.Succeeded;
		}

		public async Task<List<User>> GetUsersWithNotificationSettingsAsync()
		{
			return await _userManager.Users
				.Include(user => user.NotificationSettings)
				.ToListAsync();
		}

		public async Task<User?> GetCurrentUserAsync()
		{
			var currentUserId = GetCurrentUserId();
			if (currentUserId == null)
				return null;

			var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
			return currentUser;
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