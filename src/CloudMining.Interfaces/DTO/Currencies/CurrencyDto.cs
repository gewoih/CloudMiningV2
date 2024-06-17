using CloudMining.Domain.Enums;

namespace CloudMining.Interfaces.DTO.Currencies;

public record CurrencyDto(string ShortName, CurrencyCode Code, int Precision);