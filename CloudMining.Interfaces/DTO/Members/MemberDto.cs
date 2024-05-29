using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.DTO.Users;

namespace CloudMining.Interfaces.DTO.Members;

public record MemberDto(UserDto User, decimal Amount, decimal Share, CurrencyDto Currency, DateTime RegistrationDate);