namespace VitalGest.Core.Entities;

/// <summary>
/// Prescrição/receita médica emitida para o paciente.
/// Contém medicamentos com dosagem, frequência, duração e validade.
/// </summary>
public class Prescription
{
    public int Id { get; set; }

    /// <summary>Clínica onde a prescrição foi emitida (tenant)</summary>
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    /// <summary>Paciente que recebeu a prescrição</summary>
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    /// <summary>Médico que prescreveu</summary>
    public int DoctorUserId { get; set; }
    public User Doctor { get; set; } = null!;

    /// <summary>Agendamento associado (opcional)</summary>
    public int? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    /// <summary>Data de emissão da prescrição</summary>
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;

    /// <summary>Data de validade da prescrição (padrão: 30 dias)</summary>
    public DateOnly? ValidUntil { get; set; }

    /// <summary>Observações gerais da prescrição</summary>
    public string? Notes { get; set; }

    /// <summary>Prescrição possui assinatura digital?</summary>
    public bool IsDigitalSignature { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
}