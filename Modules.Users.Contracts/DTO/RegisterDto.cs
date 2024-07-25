namespace Modules.Users.Contracts.DTO;

public record RegisterDto(string FirstName, string LastName, string Patronymic, string Email, string Password);