using Modules.Users.Contracts.DTO;
using Modules.Users.Domain.Models;

namespace Modules.Users.Contracts.Interfaces;

public interface IUserManagementService
{
	Task<List<User>> GetUsersAsync(bool withDeposits = false, bool withShareChanges = false);
	Task<User?> GetAsync(Guid userId);
	Task<bool> UpdateAsync(User user);
	Task<User?> GetByTelegramUsernameAsync(string telegramUsername);
	Task<bool> UpdateUserSettingsAsync(Guid userId, UserSettingsDto settings);
}