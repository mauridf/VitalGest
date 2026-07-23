namespace VitalGest.Core.Entities;

/// <summary>
/// Slot de horário na agenda do profissional.
/// Gerado automaticamente a partir das regras de Schedule, pode ser reservado por um agendamento.
/// </summary>
public class TimeSlot
{
    public int Id { get; set; }

    /// <summary>Clínica do slot (tenant)</summary>
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    /// <summary>Regra de agenda que gerou este slot (opcional)</summary>
    public int? ScheduleId { get; set; }
    public Schedule? Schedule { get; set; }

    /// <summary>Médico/profissional dono do slot</summary>
    public int DoctorUserId { get; set; }
    public User Doctor { get; set; } = null!;

    /// <summary>Data do slot</summary>
    public DateOnly Date { get; set; }

    /// <summary>Horário de início do slot</summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>Horário de término do slot</summary>
    public TimeOnly EndTime { get; set; }

    /// <summary>Slot disponível para agendamento?</summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>Agendamento que reservou este slot (opcional)</summary>
    public int? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}