using CloudMining.Domain.Models.Payments;
using CloudMining.Interfaces.DTO.Members;

namespace CloudMining.Application.Mappings;

public class MemberDepositMapper : IMapper<Deposit, MemberDepositDto>
{
    public MemberDepositDto ToDto(Deposit model)
    {
        return new MemberDepositDto(model.Id, model.Date, model.Amount, model.Caption);
    }

    public Deposit ToDomain(MemberDepositDto dto)
    {
        throw new NotImplementedException();
    }
}