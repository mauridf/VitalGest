namespace VitalGest.Core.Entities;

/// <summary>
/// Exceção na agenda do profissional.
/// Usada para bloquear horários (folgas, licenças) ou adicionar horários extras.
/// Tem precedência sobre as regras regulares de Schedule.
/// </summary>
public class ScheduleException
{
    public int Id { get; set; }

    /// <summary>Clínica onde a exceção se aplica (tenant)</summary>
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    /// <summary>Médico/profissional afetado pela exceção</summary>
    public int DoctorUserId { get; set; }
    public User Doctor { get; set; } = null!;

    /// <summary>Data da exceção</summary>
    public DateOnly ExceptionDate { get; set; }

    /// <summary>Horário de início (opcional — se nulo, afeta o dia todo)</summary>
    public TimeOnly? StartTime { get; set; }

    /// <summary>Horário de fim (opcional)</summary>
    public TimeOnly? EndTime { get; set; }

    /// <summary>Motivo da exceção (folga, feriado, plantão, etc.)</summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>Se true, disponibiliza horários extras; se false, bloqueia horários</summary>
    public bool IsAvailable { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}