using System.Security.Claims;
using CloudMining.Domain.Enums;
using CloudMining.Interfaces.DTO.File;
using CloudMining.Interfaces.DTO.Users;
using CloudMining.Interfaces.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CloudMining.Application.Services;

public sealed class CurrentUserService : ICurrentUserService
{
	private readonly JwtService _jwtService;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IUserManagementService _userManagementService;
	private readonly IStorageService _storageService;

	public CurrentUserService(IUserManagementService userManagementService,
		IHttpContextAccessor httpContextAccessor,
		JwtService jwtService,
		IStorageService storageService)
	{
		_userManagementService = userManagementService;
		_httpContextAccessor = httpContextAccessor;
		_jwtService = jwtService;
		_storageService = storageService;
	}

	public async Task<string> ChangeAvatarAsync(FileDto dto)
	{
		var currentUserId = GetCurrentUserId();
		if (!currentUserId.HasValue)
			return string.Empty;

		var currentUser = await _userManagementService.GetAsync(currentUserId.Value);
		if (currentUser is null)
			return string.Empty;

		var savedFilePath = await _storageService.SaveFileAsync(dto);
		currentUser.AvatarPath = savedFilePath;

		var isUpdated = await _userManagementService.UpdateAsync(currentUser);
		return isUpdated ? savedFilePath : string.Empty;
	}

	public async Task<bool> ChangeTelegramChatIdAsync(string telegramUsername, long telegramChatId)
	{
		var user = await _userManagementService.GetByTelegramUsernameAsync(telegramUsername);
		if (user is null)
			return false;

		user.TelegramChatId = telegramChatId;
		var isUpdated = await _userManagementService.UpdateAsync(user);
		return isUpdated;
	}

	public async Task<bool> ChangeUserSettings(UserSettingsDto settings)
	{
		var currentUserId = GetCurrentUserId();
		if (!currentUserId.HasValue)
			return false;

		return await _userManagementService.UpdateUserSettingsAsync(currentUserId.Value, settings);
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