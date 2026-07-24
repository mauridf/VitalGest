using FluentValidation;
using VitalGest.Application.DTOs.Appointments;

namespace VitalGest.Application.Validators.Appointments;

public class CreateAppointmentRequestValidator : AbstractValidator<CreateAppointmentRequest>
{
    public CreateAppointmentRequestValidator()
    {
        RuleFor(x => x.PatientId)
            .GreaterThan(0).WithMessage("Paciente é obrigatório.");

        RuleFor(x => x.DoctorUserId)
            .GreaterThan(0).WithMessage("Médico é obrigatório.");

        RuleFor(x => x.AppointmentDate)
            .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Data do agendamento deve ser hoje ou uma data futura.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Horário de início é obrigatório.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Horário de término é obrigatório.")
            .Must((request, endTime) => endTime > request.StartTime)
            .WithMessage("Horário de término deve ser após o horário de início.");
    }
}