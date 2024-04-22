using CloudMining.Application.DTO.Users;
using Microsoft.AspNetCore.Identity;

namespace CloudMining.Application.Services.Users
{
	public interface IUserService
	{
		Task<IdentityResult> RegisterAsync(RegisterDto dto);
		Task<SignInResult> LoginAsync(LoginDto credentials);
        Task<List<Guid>> GetAllUsersIdsAsync();
        Guid? GetCurrentUserId();
	}
}
