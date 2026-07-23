using VitalGest.Core.Enums;

namespace VitalGest.Core.Entities;

/// <summary>
/// Usuário base do sistema.
/// Pode estar vinculado a uma ou mais clínicas via ClinicUser.
/// </summary>
public class User
{
    public int Id { get; set; }

    /// <summary>Nome de usuário único para login</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>E-mail único do usuário</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Hash da senha (BCrypt)</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Nome completo do usuário</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>CPF do usuário (opcional, único se informado)</summary>
    public string? CPF { get; set; }

    /// <summary>Telefone de contato</summary>
    public string? Phone { get; set; }

    /// <summary>URL do avatar/foto do perfil</summary>
    public string? AvatarUrl { get; set; }

    /// <summary>Role do usuário no sistema</summary>
    public UserRole Role { get; set; } = UserRole.User;

    /// <summary>Refresh token para renovação do JWT</summary>
    public string? RefreshToken { get; set; }

    /// <summary>Data de expiração do refresh token</summary>
    public DateTime? RefreshTokenExpiryTime { get; set; }

    /// <summary>Usuário ativo? (soft delete)</summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Relacionamentos
    public ICollection<ClinicUser> ClinicUsers { get; set; } = new List<ClinicUser>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Appointment> AppointmentsCreated { get; set; } = new List<Appointment>();
    public ICollection<MedicalRecordEntry> MedicalRecordEntries { get; set; } = new List<MedicalRecordEntry>();
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public ICollection<Atest> Atests { get; set; } = new List<Atest>();
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    public ICollection<ScheduleException> ScheduleExceptions { get; set; } = new List<ScheduleException>();
    public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
    public ICollection<Payment> PaymentsReceived { get; set; } = new List<Payment>();
    public ICollection<Document> DocumentsUploaded { get; set; } = new List<Document>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    public ICollection<ExamResult> ExamResultsPerformed { get; set; } = new List<ExamResult>();
    public ICollection<ExamResult> ExamResultsReviewed { get; set; } = new List<ExamResult>();
}