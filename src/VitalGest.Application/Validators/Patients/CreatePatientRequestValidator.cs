using FluentValidation;
using VitalGest.Application.DTOs.Patients;

namespace VitalGest.Application.Validators.Patients;

public class CreatePatientRequestValidator : AbstractValidator<CreatePatientRequest>
{
    public CreatePatientRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome do paciente é obrigatório.")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres.")
            .MaximumLength(255).WithMessage("Nome deve ter no máximo 255 caracteres.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefone é obrigatório.")
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres.");

        When(x => !string.IsNullOrEmpty(x.Email), () =>
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("E-mail inválido.")
                .MaximumLength(255);
        });

        When(x => !string.IsNullOrEmpty(x.CPF), () =>
        {
            RuleFor(x => x.CPF)
                .MaximumLength(14).WithMessage("CPF deve ter no máximo 14 caracteres.");
        });
    }
}