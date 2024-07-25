using CloudMining.Common.DTO;
using Modules.Users.Contracts.DTO;

namespace Modules.Users.Contracts.Interfaces;

public interface ICurrentUserService
{
	Task<string> ChangeAvatarAsync(FileDto dto);
	Task<bool> ChangeTelegramChatIdAsync(string telegramUsername, long telegramChatId);
	Task<bool> ChangeUserSettings(UserSettingsDto settings);
	Guid? GetCurrentUserId();
	bool IsCurrentUserAdmin();
}