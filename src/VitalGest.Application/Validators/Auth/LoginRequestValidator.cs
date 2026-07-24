using FluentValidation;
using VitalGest.Application.DTOs.Auth;

namespace VitalGest.Application.Validators.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Email ou username é obrigatório.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória.");
    }
}