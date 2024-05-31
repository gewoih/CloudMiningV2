using CloudMining.Interfaces.DTO.Users;

namespace CloudMining.Interfaces.DTO.Members;

public record MemberDto(UserDto User, decimal Amount, decimal? Share, DateTime? RegistrationDate);