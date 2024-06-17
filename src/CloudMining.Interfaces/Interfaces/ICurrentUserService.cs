using CloudMining.Interfaces.DTO.File;
using CloudMining.Interfaces.DTO.Users;

namespace CloudMining.Interfaces.Interfaces;

public interface ICurrentUserService
{
	Task<string> ChangeAvatarAsync(FileDto dto);
	Task<bool> ChangeTelegramChatIdAsync(string telegramUsername, long telegramChatId);
	Task<bool> ChangeUserSettings(UserSettingsDto settings);
	Guid? GetCurrentUserId();
	bool IsCurrentUserAdmin();
}