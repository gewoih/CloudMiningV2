using System.Security.Claims;
using CloudMining.Application.Mappings;
using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Identity;
using CloudMining.Domain.Models.UserSettings;
using CloudMining.Infrastructure.Database;
using CloudMining.Interfaces.DTO.File;
using CloudMining.Interfaces.DTO.NotificationSettings;
using CloudMining.Interfaces.DTO.Users;
using CloudMining.Interfaces.Interfaces;
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
		private readonly CloudMiningContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IMapper<NotificationSettings, NotificationSettingsDto> _notificationSettingsMapper;
		
		public UserService(UserManager<User> userManager,
			SignInManager<User> signInManager,
			JwtService jwtService,
			IHttpContextAccessor httpContextAccessor, 
			IStorageService storageService,
			CloudMiningContext context, 
			IMapper<NotificationSettings, NotificationSettingsDto> notificationSettingsMapper)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_httpContextAccessor = httpContextAccessor;
			_storageService = storageService;
			_context = context;
			_notificationSettingsMapper = notificationSettingsMapper;
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

		public async Task<bool> ChangeUserSettings(UserSettingsDto settings)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var currentUser = await GetCurrentUserAsync();
				if (currentUser is null)
					return false;
				
				if (settings.NotificationSettings is not null)
					await UpdateNotificationSettings(currentUser, settings.NotificationSettings);
				
				if (!string.IsNullOrEmpty(settings.TelegramUsername))
					currentUser.TelegramUsername = settings.TelegramUsername;
				
				await _context.SaveChangesAsync();

				await transaction.CommitAsync();
			}
			catch
			{
				await transaction.RollbackAsync();
				return false;
			}

			return true;
		}
		
		public async Task<User?> GetAsync(Guid userId)
		{
			return await _context.Users.FindAsync(userId);
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

		private async Task<User?> GetCurrentUserAsync()
		{
			var currentUserId = GetCurrentUserId();
			if (currentUserId == null)
				return null;

			var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
			return currentUser;
		}
		
		public bool IsCurrentUserAdmin()
		{
			var currentUserRoles = GetCurrentUserRoles();
			return currentUserRoles.Contains(UserRole.Admin);
		}

		private async Task UpdateNotificationSettings(User user, NotificationSettingsDto settings)
		{
			var currentUserSettings = await _context.NotificationSettings.FirstOrDefaultAsync(settings => 
				settings.UserId == user.Id);

			if (currentUserSettings is null)
			{
				currentUserSettings = _notificationSettingsMapper.ToDomain(settings);
				await _context.NotificationSettings.AddAsync(currentUserSettings);
			}
			else
			{
				//TODO: Возможно ли сделать через маппер?
				currentUserSettings.IsTelegramNotificationsEnabled = settings.IsTelegramNotificationsEnabled;
				currentUserSettings.NewPayoutNotification = settings.NewPayoutNotification;
				currentUserSettings.NewPurchaseNotification = settings.NewPurchaseNotification;
				currentUserSettings.NewElectricityPaymentNotification = settings.NewElectricityPaymentNotification;
				currentUserSettings.UnpaidElectricityPaymentReminder = settings.UnpaidElectricityPaymentReminder;
				currentUserSettings.UnpaidPurchasePaymentReminder = settings.UnpaidPurchasePaymentReminder;
			}
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