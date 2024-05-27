namespace CloudMining.Interfaces.DTO.Users;

public record RegisterDto(string FirstName, string LastName, string Patronymic, string Email, string Password);