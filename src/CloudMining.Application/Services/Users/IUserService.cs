using CloudMining.Application.DTO.Users;
using CloudMining.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace CloudMining.Application.Services.Users
{
	public interface IUserService
	{
		Task<IdentityResult> RegisterAsync(RegisterDto dto);
		Task<string> LoginAsync(LoginDto credentials);
        Guid? GetCurrentUserId();
        IEnumerable<UserRole> GetCurrentUserRoles();
        bool IsCurrentUserAdmin();
	}
}
