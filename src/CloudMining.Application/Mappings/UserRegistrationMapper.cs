using CloudMining.Interfaces.DTO.Users;
using CloudMining.Domain.Models.Identity;

namespace CloudMining.Application.Mappings;

public class UserRegistrationMapper : IMapper<User, RegisterDto>
{
    public RegisterDto ToDto(User model)
    {
        return new RegisterDto(model.FirstName, model.LastName, model.Patronymic, model.Email, model.PasswordHash);
    }

    public User ToDomain(RegisterDto dto)
    {
        return new User
        {
            Email = dto.Email,
            UserName = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Patronymic = dto.Patronymic
        };
    }
}