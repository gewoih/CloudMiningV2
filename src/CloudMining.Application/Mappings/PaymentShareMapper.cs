using CloudMining.Application.DTO.Payments.Admin;
using CloudMining.Application.DTO.Users;
using CloudMining.Domain.Models;
using CloudMining.Domain.Models.Identity;

namespace CloudMining.Application.Mappings;

public class PaymentShareMapper : IMapper<PaymentShare, PaymentShareDto>
{
    public PaymentShareDto ToDto(PaymentShare model)
    {
        return new PaymentShareDto
        {
            User = new UserDto
            {
                Id = model.User.Id,
                FirstName = model.User.FirstName,
                LastName = model.User.LastName,
                Patronymic = model.User.Patronymic
            },
            
            Amount = model.Amount,
            Share = model.Share,
            IsCompleted = model.IsCompleted
        };
    }

    public PaymentShare ToDomain(PaymentShareDto dto)
    {
        return new PaymentShare
        {
            User = new User
            {
                Id = dto.User.Id,
                FirstName = dto.User.FirstName,
                LastName = dto.User.LastName,
                Patronymic = dto.User.Patronymic
            },
            
            Amount = dto.Amount,
            Share = dto.Share,
            IsCompleted = dto.IsCompleted
        };
    }
}