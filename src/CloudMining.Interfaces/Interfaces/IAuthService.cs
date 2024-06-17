using CloudMining.Interfaces.DTO.Users;
using Microsoft.AspNetCore.Identity;

namespace CloudMining.Interfaces.Interfaces;

public interface IAuthService
{
	Task<IdentityResult> RegisterAsync(RegisterDto dto);
	Task<string> LoginAsync(LoginDto credentials);
	Task<bool> ChangeEmailAsync(ChangeEmailDto dto);
	Task<bool> ChangePasswordAsync(ChangePasswordDto dto);
}