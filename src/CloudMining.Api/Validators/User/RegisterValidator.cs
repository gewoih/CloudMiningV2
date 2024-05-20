using CloudMining.Interfaces.DTO.Users;
using FluentValidation;

namespace CloudMining.Api.Validators.User;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("Имя не может быть пустым");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Фамилия не может быть пустой");
        RuleFor(x => x.Patronymic).NotEmpty().WithMessage("Отчество не может быть пустым");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Имя не может быть пустым");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email не может быть пустым")
            .EmailAddress().WithMessage("Некорректный формат Email");
    }
}