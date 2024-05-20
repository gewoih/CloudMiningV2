using CloudMining.Domain.Models;
using CloudMining.Domain.Models.Payments;
using CloudMining.Interfaces.DTO.Payments.Deposits;

namespace CloudMining.Application.Mappings;

public class DepositMapper : IMapper<Deposit, CreateDepositDto>
{
    public CreateDepositDto ToDto(Deposit model)
    {
        return new CreateDepositDto
        {
            UserId = model.UserId,
            Amount = model.Amount,
            Date = model.Date
        };
    }

    public Deposit ToDomain(CreateDepositDto dto)
    {
        return new Deposit
        {
            UserId = dto.UserId,
            Amount = dto.Amount,
            Date = dto.Date
        };
    }
}