namespace VitalGest.Core.Entities;

/// <summary>
/// Atestado médico emitido para o paciente.
/// Contém CID, período de afastamento e dias de repouso.
/// </summary>
public class Atest
{
    public int Id { get; set; }

    /// <summary>Clínica onde o atestado foi emitido (tenant)</summary>
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    /// <summary>Paciente que recebeu o atestado</summary>
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    /// <summary>Médico que emitiu o atestado</summary>
    public int DoctorUserId { get; set; }
    public User Doctor { get; set; } = null!;

    /// <summary>Agendamento associado (opcional)</summary>
    public int? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    /// <summary>Data de emissão do atestado</summary>
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;

    /// <summary>Data de início do afastamento</summary>
    public DateOnly StartDate { get; set; }

    /// <summary>Data de fim do afastamento</summary>
    public DateOnly EndDate { get; set; }

    /// <summary>Código CID da doença/condição (opcional)</summary>
    public string? CID { get; set; }

    /// <summary>Descrição do atestado/motivo do afastamento</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Quantidade de dias de repouso</summary>
    public int RestDays { get; set; }

    /// <summary>Atestado possui assinatura digital?</summary>
    public bool IsDigitalSignature { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}