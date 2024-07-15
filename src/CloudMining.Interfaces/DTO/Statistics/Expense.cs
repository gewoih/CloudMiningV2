using CloudMining.Domain.Enums;

namespace CloudMining.Interfaces.DTO.Statistics;

public record Expense(ExpenseType Type, List<PriceBar> PriceBars);