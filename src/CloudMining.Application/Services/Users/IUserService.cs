using CloudMining.Application.DTO.Users;
using Microsoft.AspNetCore.Identity;

namespace CloudMining.Application.Services.Users
{
	public interface IUserService
	{
		Task<IdentityResult> RegisterAsync(RegisterDto dto);
		Task<string> LoginAsync(LoginDto credentials);
        Task<bool> ChangeEmailAsync(ChangeEmailDto dto);
        Guid? GetCurrentUserId();
        bool IsCurrentUserAdmin();
	}
}
