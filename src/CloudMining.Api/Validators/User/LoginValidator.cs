using CloudMining.Interfaces.DTO.Users;
using FluentValidation;

namespace CloudMining.Api.Validators.User;

public class LoginValidator : AbstractValidator<LoginDto>
{
	public LoginValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email не может быть пустым")
			.NotEmpty().EmailAddress().WithMessage("Некорректный формат Email");

		RuleFor(x => x.Password).NotEmpty().WithMessage("Пароль не может быть пустым");
	}
}