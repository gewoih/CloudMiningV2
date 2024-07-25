using CloudMining.Common.Mappers;
using CloudMining.Common.Models.Identity;
using Modules.Payments.Contracts.DTO.User;
using Modules.Payments.Contracts.Interfaces;

namespace Modules.Payments.Application.Mappers;

public class MemberMapper : IMapper<User, MemberDto>
{
    private readonly IShareService _shareService;

    public MemberMapper(IShareService shareService)
    {
        _shareService = shareService;
    }

    public MemberDto ToDto(User model)
    {
        var userShare = _shareService.CalculateUserShare(model.ShareChanges);
        var totalAmount = model.Deposits.Sum(deposit => deposit.Amount);
        var user = new UserDto(model.Id, model.FirstName, model.LastName, model.Patronymic);
        
        return new MemberDto(user, totalAmount, userShare, model.RegistrationDate);
    }

    public User ToDomain(MemberDto dto)
    {
        throw new NotImplementedException();
    }
}