using CloudMining.Contracts.DTO.File;
using CloudMining.Contracts.DTO.Users;
using CloudMining.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace CloudMining.Contracts.Interfaces
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
        Task<User?> GetCurrentUserAsync();
        Guid? GetCurrentUserId();
        bool IsCurrentUserAdmin();
	}
}
