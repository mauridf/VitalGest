using FluentValidation;
using VitalGest.Application.DTOs.Auth;

namespace VitalGest.Application.Validators.Auth;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Senha atual é obrigatória.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Nova senha é obrigatória.")
            .MinimumLength(8).WithMessage("Nova senha deve ter no mínimo 8 caracteres.")
            .Matches("[A-Z]").WithMessage("Nova senha deve conter pelo menos uma letra maiúscula.")
            .Matches("[a-z]").WithMessage("Nova senha deve conter pelo menos uma letra minúscula.")
            .Matches("[0-9]").WithMessage("Nova senha deve conter pelo menos um número.")
            .NotEqual(x => x.CurrentPassword).WithMessage("Nova senha deve ser diferente da senha atual.");

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword).WithMessage("As senhas não conferem.");
    }
}