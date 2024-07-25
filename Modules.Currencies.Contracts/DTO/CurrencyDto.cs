using Modules.Currencies.Domain.Enums;

namespace Modules.Currencies.Contracts.DTO;

public record CurrencyDto(string ShortName, CurrencyCode Code, int Precision);