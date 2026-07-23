using VitalGest.Core.Entities;

namespace VitalGest.Core.Interfaces;

/// <summary>
/// Padrão Unit of Work para controle transacional e acesso centralizado a repositórios.
/// Garante atomicidade em operações que envolvem múltiplos repositórios.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // ===== Repositórios Específicos =====
    IUserRepository Users { get; }
    IPatientRepository Patients { get; }
    IAppointmentRepository Appointments { get; }
    IClinicRepository Clinics { get; }
    IMedicalRecordRepository MedicalRecords { get; }
    IExamRepository Exams { get; }
    IPrescriptionRepository Prescriptions { get; }
    IScheduleRepository Schedules { get; }

    // ===== Repositórios Genéricos =====
    IRepository<Department> Departments { get; }
    IRepository<Position> Positions { get; }
    IRepository<Specialty> Specialties { get; }
    IRepository<Payment> Payments { get; }
    IRepository<Invoice> Invoices { get; }
    IRepository<InsurancePlan> InsurancePlans { get; }
    IRepository<Document> Documents { get; }
    IRepository<Notification> Notifications { get; }
    IRepository<AuditLog> AuditLogs { get; }
    IRepository<Atest> Atests { get; }
    IRepository<ExamType> ExamTypes { get; }
    IRepository<TimeSlot> TimeSlots { get; }
    IRepository<ScheduleException> ScheduleExceptions { get; }
    IRepository<WaitingRoomEntry> WaitingRoomEntries { get; }
    IRepository<MedicalRecordEntry> MedicalRecordEntries { get; }
    IRepository<PrescriptionItem> PrescriptionItems { get; }
    IRepository<ExamResult> ExamResults { get; }
    IRepository<InsuranceCoverage> InsuranceCoverages { get; }
    IRepository<ProcedureType> ProcedureTypes { get; }

    // ===== Métodos Transacionais =====
    /// <summary>Salva todas as alterações pendentes</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>Inicia uma transação explícita</summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Confirma a transação atual</summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Desfaz a transação atual</summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}