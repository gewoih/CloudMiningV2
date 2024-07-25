namespace Modules.Payments.Contracts.DTO.User;

public record MemberDto(UserDto User, decimal Amount, decimal? Share, DateTime? RegistrationDate);