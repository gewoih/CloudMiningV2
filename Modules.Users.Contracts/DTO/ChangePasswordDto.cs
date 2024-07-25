namespace Modules.Users.Contracts.DTO;

public record ChangePasswordDto(string CurrentPassword, string NewPassword);