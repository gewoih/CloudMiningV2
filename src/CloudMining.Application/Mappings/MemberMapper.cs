using CloudMining.Domain.Enums;
using CloudMining.Domain.Models.Identity;
using CloudMining.Interfaces.DTO.Currencies;
using CloudMining.Interfaces.DTO.Members;
using CloudMining.Interfaces.DTO.Users;
using CloudMining.Interfaces.Interfaces;

namespace CloudMining.Application.Mappings;

public class MemberMapper : IMapper<User, MemberDto>
{
    public MemberDto ToDto(User model)
    {
        var currentMemberShare =
            model.ShareChanges
                .MaxBy(shareChange => shareChange.CreatedDate)
                ?.After;
        var totalAmount = model.Deposits
            .Sum(deposit => deposit.Amount);
        var user = new UserDto(model.Id, model.FirstName, model.LastName, model.Patronymic);
        return new MemberDto(user, totalAmount, currentMemberShare, model.RegistrationDate);
    }

    public User ToDomain(MemberDto dto)
    {
        throw new NotImplementedException();
    }
}