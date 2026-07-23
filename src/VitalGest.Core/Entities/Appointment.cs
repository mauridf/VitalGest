using VitalGest.Core.Enums;

namespace VitalGest.Core.Entities;

/// <summary>
/// Agendamento de consulta, exame, retorno ou procedimento.
/// Associa paciente, médico, data/horário e mantém o status do atendimento.
/// </summary>
public class Appointment
{
    public int Id { get; set; }

    /// <summary>Clínica onde o agendamento foi realizado (tenant)</summary>
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    /// <summary>Paciente agendado</summary>
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    /// <summary>Médico/profissional responsável pelo atendimento</summary>
    public int DoctorUserId { get; set; }
    public User Doctor { get; set; } = null!;

    /// <summary>Departamento onde será realizado o atendimento</summary>
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    /// <summary>Especialidade do atendimento</summary>
    public int? SpecialtyId { get; set; }
    public Specialty? Specialty { get; set; }

    /// <summary>Data da consulta/exame</summary>
    public DateOnly AppointmentDate { get; set; }

    /// <summary>Horário de início</summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>Horário de término</summary>
    public TimeOnly EndTime { get; set; }

    /// <summary>Status do agendamento</summary>
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    /// <summary>Tipo de agendamento (consulta, exame, retorno, procedimento)</summary>
    public AppointmentType Type { get; set; } = AppointmentType.Consultation;

    /// <summary>Observações gerais sobre o agendamento</summary>
    public string? Notes { get; set; }

    /// <summary>Observações internas (não visíveis ao paciente)</summary>
    public string? InternalNotes { get; set; }

    /// <summary>Agendamento confirmado pelo paciente?</summary>
    public bool IsConfirmed { get; set; }

    /// <summary>Data/hora da confirmação</summary>
    public DateTime? ConfirmedAt { get; set; }

    /// <summary>Data/hora do cancelamento</summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>Motivo do cancelamento (obrigatório se cancelado)</summary>
    public string? CancelReason { get; set; }

    /// <summary>Usuário que criou o agendamento</summary>
    public int CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Relacionamentos
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public ICollection<Atest> Atests { get; set; } = new List<Atest>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<MedicalRecordEntry> MedicalRecordEntries { get; set; } = new List<MedicalRecordEntry>();
    public ICollection<WaitingRoomEntry> WaitingRoomEntries { get; set; } = new List<WaitingRoomEntry>();
    public TimeSlot? TimeSlot { get; set; }
}