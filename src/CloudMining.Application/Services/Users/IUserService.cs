using CloudMining.Application.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace CloudMining.Application.Services.Users
{
	public interface IUserService
	{
		Task<IdentityResult> RegisterAsync(RegisterCredentials credentials);
		Task<SignInResult> LoginAsync(LoginCredentials credentials);
        Task<List<Guid>> GetAllUsersIdsAsync();

    }
}
