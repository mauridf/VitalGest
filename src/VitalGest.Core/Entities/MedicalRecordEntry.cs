using VitalGest.Core.Enums;

namespace VitalGest.Core.Entities;

/// <summary>
/// Entrada/registro no prontuário eletrônico do paciente.
/// Cada entrada representa uma evolução, prescrição, exame, atestado, receita ou observação.
/// </summary>
public class MedicalRecordEntry
{
    public int Id { get; set; }

    /// <summary>Prontuário ao qual esta entrada pertence</summary>
    public int MedicalRecordId { get; set; }
    public MedicalRecord MedicalRecord { get; set; } = null!;

    /// <summary>Agendamento associado à entrada (opcional)</summary>
    public int? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    /// <summary>Médico que registrou a entrada</summary>
    public int DoctorUserId { get; set; }
    public User Doctor { get; set; } = null!;

    /// <summary>Tipo de entrada (evolução, prescrição, exame, etc.)</summary>
    public MedicalRecordEntryType EntryType { get; set; }

    /// <summary>Descrição/conteúdo da entrada</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Entrada confidencial (visível apenas ao médico criador e admin)</summary>
    public bool IsConfidential { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}