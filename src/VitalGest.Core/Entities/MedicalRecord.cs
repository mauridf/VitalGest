namespace VitalGest.Core.Entities;

/// <summary>
/// Prontuário Eletrônico do Paciente (PEP).
/// Cada paciente possui um prontuário único por clínica, contendo entradas cronológicas.
/// </summary>
public class MedicalRecord
{
    public int Id { get; set; }

    /// <summary>Paciente titular do prontuário</summary>
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    /// <summary>Clínica onde o prontuário foi criado (tenant)</summary>
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Relacionamentos
    public ICollection<MedicalRecordEntry> Entries { get; set; } = new List<MedicalRecordEntry>();
}