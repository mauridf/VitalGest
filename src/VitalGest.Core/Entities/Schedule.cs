namespace VitalGest.Core.Entities;

/// <summary>
/// Regra de agenda do profissional.
/// Define os dias da semana, horários e duração dos slots para geração automática de horários.
/// </summary>
public class Schedule
{
    public int Id { get; set; }

    /// <summary>Clínica onde a regra de agenda se aplica (tenant)</summary>
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    /// <summary>Médico/profissional dono da agenda</summary>
    public int DoctorUserId { get; set; }
    public User Doctor { get; set; } = null!;

    /// <summary>Dia da semana (0=Domingo, 1=Segunda, ..., 6=Sábado)</summary>
    public int DayOfWeek { get; set; }

    /// <summary>Horário de início do expediente</summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>Horário de término do expediente</summary>
    public TimeOnly EndTime { get; set; }

    /// <summary>Duração de cada slot em minutos (padrão: 30)</summary>
    public int SlotDuration { get; set; } = 30;

    /// <summary>Regra ativa?</summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Relacionamentos
    public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
}