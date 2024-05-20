using CloudMining.Domain.Models.Identity;
using CloudMining.Interfaces.DTO.File;
using CloudMining.Interfaces.DTO.Users;
using Microsoft.AspNetCore.Identity;

namespace CloudMining.Interfaces.Interfaces
{
	public interface IUserService
	{
		Task<IdentityResult> RegisterAsync(RegisterDto dto);
		Task<string> LoginAsync(LoginDto credentials);
        Task<bool> ChangeEmailAsync(ChangeEmailDto dto);
        Task<bool> ChangePasswordAsync(ChangePasswordDto dto);
        Task<string> ChangeAvatarAsync(FileDto dto);
        Task<bool> ChangeTelegramChatIdAsync(string telegramUsername, long telegramChatId);
        Task<List<User>> GetUsersWithNotificationSettingsAsync();
        Task<bool> ChangeUserSettings(UserSettingsDto settings);
        Task<User?> GetCurrentUserAsync();
        Task<User?> GetAsync(Guid userId);
        Guid? GetCurrentUserId();
        bool IsCurrentUserAdmin();
	}
}
