using Modules.Notifications.Contracts.DTO;

namespace Modules.Users.Contracts.DTO;

public record UserSettingsDto(string? TelegramUsername, NotificationSettingsDto? NotificationSettings);