using FluentValidation;
using VitalGest.Application.DTOs.Auth;

namespace VitalGest.Application.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username é obrigatório.")
            .MinimumLength(3).WithMessage("Username deve ter no mínimo 3 caracteres.")
            .MaximumLength(100).WithMessage("Username deve ter no máximo 100 caracteres.")
            .Matches("^[a-zA-Z0-9_.-]+$").WithMessage("Username deve conter apenas letras, números, pontos, underscores e hífens.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.")
            .MaximumLength(255).WithMessage("E-mail deve ter no máximo 255 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória.")
            .MinimumLength(8).WithMessage("Senha deve ter no mínimo 8 caracteres.")
            .Matches("[A-Z]").WithMessage("Senha deve conter pelo menos uma letra maiúscula.")
            .Matches("[a-z]").WithMessage("Senha deve conter pelo menos uma letra minúscula.")
            .Matches("[0-9]").WithMessage("Senha deve conter pelo menos um número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Senha deve conter pelo menos um caractere especial.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("As senhas não conferem.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres.")
            .MaximumLength(255).WithMessage("Nome deve ter no máximo 255 caracteres.");

        When(x => !string.IsNullOrEmpty(x.CPF), () =>
        {
            RuleFor(x => x.CPF)
                .Length(14).WithMessage("CPF deve ter 14 caracteres (formato: 000.000.000-00).");
        });

        When(x => !string.IsNullOrEmpty(x.Phone), () =>
        {
            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres.");
        });
    }
}