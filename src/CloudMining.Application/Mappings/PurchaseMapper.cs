using CloudMining.Domain.Models.Purchases;
using CloudMining.Interfaces.DTO.Purchases;

namespace CloudMining.Application.Mappings;

public class PurchaseMapper : IMapper<Purchase, PurchaseDto>
{
	public PurchaseDto ToDto(Purchase model)
	{
		return new PurchaseDto(model.Id, model.Caption, model.Amount, model.Date);
	}

	public Purchase ToDomain(PurchaseDto dto)
	{
		return new Purchase
		{
			Id = dto.Id,
			Caption = dto.Caption,
			Amount = dto.Amount,
			Date = dto.Date
		};
	}
}