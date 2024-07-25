namespace Modules.Payments.Contracts.DTO;

public record PaymentsPageDto(List<PaymentDto> Items, int TotalCount);