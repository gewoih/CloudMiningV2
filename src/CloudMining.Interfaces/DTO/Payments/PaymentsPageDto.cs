namespace CloudMining.Interfaces.DTO.Payments;

public record PaymentsPageDto(List<PaymentDto> Items, int TotalCount);