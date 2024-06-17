namespace CloudMining.Interfaces.DTO.Users;

public record ChangePasswordDto(string CurrentPassword, string NewPassword);