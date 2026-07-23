using VitalGest.Core.Enums;

namespace VitalGest.Core.Entities;

/// <summary>
/// Paciente cadastrado em uma clínica.
/// Um paciente pode estar em múltiplas clínicas (registros independentes).
/// </summary>
public class Patient
{
    public int Id { get; set; }

    /// <summary>Clínica onde está cadastrado (tenant)</summary>
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    /// <summary>Nome completo do paciente</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>CPF (único globalmente se informado)</summary>
    public string? CPF { get; set; }

    /// <summary>RG do paciente</summary>
    public string? RG { get; set; }

    /// <summary>Data de nascimento</summary>
    public DateOnly? BirthDate { get; set; }

    /// <summary>Gênero</summary>
    public Gender? Gender { get; set; }

    /// <summary>Telefone principal</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>Telefone secundário</summary>
    public string? SecondaryPhone { get; set; }

    /// <summary>E-mail do paciente</summary>
    public string? Email { get; set; }

    /// <summary>Endereço do paciente</summary>
    public int? AddressId { get; set; }
    public Address? Address { get; set; }

    /// <summary>Tipo sanguíneo</summary>
    public BloodType? BloodType { get; set; }

    /// <summary>Alergias conhecidas (campo crítico de segurança)</summary>
    public string? Allergies { get; set; }

    /// <summary>Observações médicas relevantes (campo crítico)</summary>
    public string? MedicalNotes { get; set; }

    /// <summary>Nome do contato de emergência</summary>
    public string? EmergencyContact { get; set; }

    /// <summary>Telefone do contato de emergência</summary>
    public string? EmergencyPhone { get; set; }

    /// <summary>Convênio/plano de saúde</summary>
    public int? InsurancePlanId { get; set; }
    public InsurancePlan? InsurancePlan { get; set; }

    /// <summary>Número da carteirinha do convênio</summary>
    public string? InsuranceCardNumber { get; set; }

    /// <summary>Data de validade da carteirinha</summary>
    public DateOnly? InsuranceExpiryDate { get; set; }

    /// <summary>URL da foto de perfil do paciente</summary>
    public string? ProfilePhotoUrl { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Relacionamentos
    public MedicalRecord? MedicalRecord { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public ICollection<Atest> Atests { get; set; } = new List<Atest>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<WaitingRoomEntry> WaitingRoomEntries { get; set; } = new List<WaitingRoomEntry>();
}