using Microsoft.AspNetCore.Identity;
using Modules.Users.Contracts.DTO;

namespace Modules.Users.Contracts.Interfaces;

public interface IAuthService
{
	Task<IdentityResult> RegisterAsync(RegisterDto dto);
	Task<string> LoginAsync(LoginDto credentials);
	Task<bool> ChangeEmailAsync(ChangeEmailDto dto);
	Task<bool> ChangePasswordAsync(ChangePasswordDto dto);
}