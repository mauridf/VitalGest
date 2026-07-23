using VitalGest.Core.Enums;

namespace VitalGest.Core.Entities;

/// <summary>
/// Solicitação de exame laboratorial ou de imagem.
/// Acompanha o fluxo desde a solicitação até a entrega do resultado.
/// </summary>
public class Exam
{
    public int Id { get; set; }

    /// <summary>Clínica onde o exame foi solicitado (tenant)</summary>
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    /// <summary>Paciente que realizou o exame</summary>
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    /// <summary>Médico que solicitou o exame</summary>
    public int DoctorUserId { get; set; }
    public User Doctor { get; set; } = null!;

    /// <summary>Tipo de exame solicitado</summary>
    public int ExamTypeId { get; set; }
    public ExamType ExamType { get; set; } = null!;

    /// <summary>Agendamento associado (opcional)</summary>
    public int? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    /// <summary>Data da solicitação do exame</summary>
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;

    /// <summary>Status atual do exame</summary>
    public ExamStatus Status { get; set; } = ExamStatus.Requested;

    /// <summary>Observações do médico solicitante</summary>
    public string? Notes { get; set; }

    /// <summary>História clínica relevante para o exame</summary>
    public string? ClinicalHistory { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Relacionamentos
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ExamResult? Result { get; set; }
}