using CloudMining.Application.DTO.Users;
using CloudMining.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace CloudMining.Application.Services.Users
{
	public interface IUserService
	{
		Task<IdentityResult> RegisterAsync(RegisterDto dto);
		Task<string> LoginAsync(LoginDto credentials);
        Task<List<Guid>> GetAllUsersIdsAsync();
        Guid? GetCurrentUserId();
        Task<User?> GetCurrentUserAsync();
	}
}
