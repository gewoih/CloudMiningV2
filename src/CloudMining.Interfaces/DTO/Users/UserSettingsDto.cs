using CloudMining.Interfaces.DTO.NotificationSettings;

namespace CloudMining.Interfaces.DTO.Users;

public record UserSettingsDto(string? TelegramUsername, NotificationSettingsDto? NotificationSettings);