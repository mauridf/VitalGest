namespace VitalGest.Core.Entities;

/// <summary>
/// Clínica (tenant principal do sistema).
/// Cada clínica é um tenant isolado com seus próprios dados.
/// </summary>
public class Clinic
{
    public int Id { get; set; }

    /// <summary>Nome fantasia da clínica</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Razão social</summary>
    public string CorporateName { get; set; } = string.Empty;

    /// <summary>CNPJ (formato: XX.XXX.XXX/XXXX-XX)</summary>
    public string CNPJ { get; set; } = string.Empty;

    /// <summary>Descrição/especialidades da clínica</summary>
    public string? Description { get; set; }

    /// <summary>URL do logo da clínica</summary>
    public string? LogoUrl { get; set; }

    /// <summary>Telefone principal para contato</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>Telefone secundário (opcional)</summary>
    public string? SecondaryPhone { get; set; }

    /// <summary>E-mail de contato da clínica</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Website da clínica</summary>
    public string? Website { get; set; }

    /// <summary>Endereço da clínica</summary>
    public int? AddressId { get; set; }
    public Address? Address { get; set; }

    /// <summary>Horários de funcionamento (JSON: {"seg":"08:00-18:00", ...})</summary>
    public string? OpeningHours { get; set; }

    /// <summary>Clínica ativa? (soft delete)</summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Relacionamentos
    public ICollection<Department> Departments { get; set; } = new List<Department>();
    public ICollection<ClinicUser> ClinicUsers { get; set; } = new List<ClinicUser>();
    public ICollection<Patient> Patients { get; set; } = new List<Patient>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    public ICollection<ScheduleException> ScheduleExceptions { get; set; } = new List<ScheduleException>();
    public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public ICollection<Atest> Atests { get; set; } = new List<Atest>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    public ICollection<WaitingRoomEntry> WaitingRoomEntries { get; set; } = new List<WaitingRoomEntry>();
}