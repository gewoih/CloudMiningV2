using CloudMining.Application.DTO.File;
using CloudMining.Application.DTO.Users;
using CloudMining.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace CloudMining.Application.Services.Users
{
	public interface IUserService
	{
		Task<IdentityResult> RegisterAsync(RegisterDto dto);
		Task<string> LoginAsync(LoginDto credentials);
        Task<bool> ChangeEmailAsync(ChangeEmailDto dto);
        Task<bool> ChangePasswordAsync(ChangePasswordDto dto);
        Task<string> ChangeAvatarAsync(FileDto dto);
        Task<List<User>> GetUsersWithNotificationSettingsAsync();
        Task<User?> GetCurrentUserAsync();
        Guid? GetCurrentUserId();
        bool IsCurrentUserAdmin();
	}
}
