using Microsoft.EntityFrameworkCore.Storage;
using VitalGest.Core.Entities;
using VitalGest.Core.Interfaces;
using VitalGest.Infrastructure.Data.Context;

namespace VitalGest.Infrastructure.Data.Repositories;

/// <summary>
/// Implementação do padrão Unit of Work.
/// Centraliza o acesso a todos os repositórios e gerencia transações.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly VitalGestDbContext _context;
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    // ===== Repositórios Específicos (Lazy Loading) =====
    private IUserRepository? _userRepository;
    private IPatientRepository? _patientRepository;
    private IAppointmentRepository? _appointmentRepository;
    private IClinicRepository? _clinicRepository;
    private IMedicalRecordRepository? _medicalRecordRepository;
    private IExamRepository? _examRepository;
    private IPrescriptionRepository? _prescriptionRepository;
    private IScheduleRepository? _scheduleRepository;

    // ===== Repositórios Genéricos (Lazy Loading) =====
    private IRepository<Department>? _departmentRepository;
    private IRepository<Position>? _positionRepository;
    private IRepository<Specialty>? _specialtyRepository;
    private IRepository<Payment>? _paymentRepository;
    private IRepository<Invoice>? _invoiceRepository;
    private IRepository<InsurancePlan>? _insurancePlanRepository;
    private IRepository<Document>? _documentRepository;
    private IRepository<Notification>? _notificationRepository;
    private IRepository<AuditLog>? _auditLogRepository;
    private IRepository<Atest>? _atestRepository;
    private IRepository<ExamType>? _examTypeRepository;
    private IRepository<TimeSlot>? _timeSlotRepository;
    private IRepository<ScheduleException>? _scheduleExceptionRepository;
    private IRepository<WaitingRoomEntry>? _waitingRoomRepository;
    private IRepository<MedicalRecordEntry>? _medicalRecordEntryRepository;
    private IRepository<PrescriptionItem>? _prescriptionItemRepository;
    private IRepository<ExamResult>? _examResultRepository;
    private IRepository<InsuranceCoverage>? _insuranceCoverageRepository;
    private IRepository<ProcedureType>? _procedureTypeRepository;

    public UnitOfWork(VitalGestDbContext context)
    {
        _context = context;
    }

    // ===== Repositórios Específicos =====
    public IUserRepository Users =>
        _userRepository ??= new UserRepository(_context);
    public IPatientRepository Patients =>
        _patientRepository ??= new PatientRepository(_context);
    public IAppointmentRepository Appointments =>
        _appointmentRepository ??= new AppointmentRepository(_context);
    public IClinicRepository Clinics =>
        _clinicRepository ??= new ClinicRepository(_context);
    public IMedicalRecordRepository MedicalRecords =>
        _medicalRecordRepository ??= new MedicalRecordRepository(_context);
    public IExamRepository Exams =>
        _examRepository ??= new ExamRepository(_context);
    public IPrescriptionRepository Prescriptions =>
        _prescriptionRepository ??= new PrescriptionRepository(_context);
    public IScheduleRepository Schedules =>
        _scheduleRepository ??= new ScheduleRepository(_context);

    // ===== Repositórios Genéricos =====
    public IRepository<Department> Departments =>
        _departmentRepository ??= new Repository<Department>(_context);
    public IRepository<Position> Positions =>
        _positionRepository ??= new Repository<Position>(_context);
    public IRepository<Specialty> Specialties =>
        _specialtyRepository ??= new Repository<Specialty>(_context);
    public IRepository<Payment> Payments =>
        _paymentRepository ??= new Repository<Payment>(_context);
    public IRepository<Invoice> Invoices =>
        _invoiceRepository ??= new Repository<Invoice>(_context);
    public IRepository<InsurancePlan> InsurancePlans =>
        _insurancePlanRepository ??= new Repository<InsurancePlan>(_context);
    public IRepository<Document> Documents =>
        _documentRepository ??= new Repository<Document>(_context);
    public IRepository<Notification> Notifications =>
        _notificationRepository ??= new Repository<Notification>(_context);
    public IRepository<AuditLog> AuditLogs =>
        _auditLogRepository ??= new Repository<AuditLog>(_context);
    public IRepository<Atest> Atests =>
        _atestRepository ??= new Repository<Atest>(_context);
    public IRepository<ExamType> ExamTypes =>
        _examTypeRepository ??= new Repository<ExamType>(_context);
    public IRepository<TimeSlot> TimeSlots =>
        _timeSlotRepository ??= new Repository<TimeSlot>(_context);
    public IRepository<ScheduleException> ScheduleExceptions =>
        _scheduleExceptionRepository ??= new Repository<ScheduleException>(_context);
    public IRepository<WaitingRoomEntry> WaitingRoomEntries =>
        _waitingRoomRepository ??= new Repository<WaitingRoomEntry>(_context);
    public IRepository<MedicalRecordEntry> MedicalRecordEntries =>
        _medicalRecordEntryRepository ??= new Repository<MedicalRecordEntry>(_context);
    public IRepository<PrescriptionItem> PrescriptionItems =>
        _prescriptionItemRepository ??= new Repository<PrescriptionItem>(_context);
    public IRepository<ExamResult> ExamResults =>
        _examResultRepository ??= new Repository<ExamResult>(_context);
    public IRepository<InsuranceCoverage> InsuranceCoverages =>
        _insuranceCoverageRepository ??= new Repository<InsuranceCoverage>(_context);
    public IRepository<ProcedureType> ProcedureTypes =>
        _procedureTypeRepository ??= new Repository<ProcedureType>(_context);

    // ===== Métodos Transacionais =====
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
            throw new InvalidOperationException("Já existe uma transação ativa.");

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
            throw new InvalidOperationException("Nenhuma transação ativa para confirmar.");

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            await DisposeTransactionAsync();
        }
    }

    private async Task DisposeTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _currentTransaction?.Dispose();
            _context.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}