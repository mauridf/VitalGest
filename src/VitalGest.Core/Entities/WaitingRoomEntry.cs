using VitalGest.Core.Enums;

namespace VitalGest.Core.Entities;

/// <summary>
/// Registro de entrada na sala de espera da clínica.
/// Controla a chegada, chamada e atendimento do paciente.
/// </summary>
public class WaitingRoomEntry
{
    public int Id { get; set; }

    /// <summary>Clínica onde o paciente aguarda (tenant)</summary>
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    /// <summary>Agendamento associado</summary>
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;

    /// <summary>Paciente na sala de espera</summary>
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    /// <summary>Hora de chegada do paciente</summary>
    public DateTime ArrivalTime { get; set; } = DateTime.UtcNow;

    /// <summary>Hora em que o paciente foi chamado</summary>
    public DateTime? CalledAt { get; set; }

    /// <summary>Hora em que o atendimento foi iniciado</summary>
    public DateTime? AttendedAt { get; set; }

    /// <summary>Status na sala de espera</summary>
    public WaitingRoomStatus Status { get; set; } = WaitingRoomStatus.Waiting;

    /// <summary>Prioridade de atendimento (1=normal, maior=prioridade)</summary>
    public int Priority { get; set; } = 1;

    /// <summary>Observações sobre a entrada</summary>
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}