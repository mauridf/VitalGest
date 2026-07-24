using Microsoft.EntityFrameworkCore;
using VitalGest.Core.Entities;
using VitalGest.Core.Interfaces;

namespace VitalGest.Infrastructure.Data.Context;

/// <summary>
/// Contexto principal do Entity Framework Core.
/// Contém todos os DbSets (35+ tabelas) e configurações de mapeamento.
/// Aplica Global Query Filter para isolamento multi-tenant.
/// </summary>
public class VitalGestDbContext : DbContext
{
    private readonly ITenantService _tenantService;

    // ===== DbSets - Tabelas Base =====
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Clinic> Clinics => Set<Clinic>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Specialty> Specialties => Set<Specialty>();
    public DbSet<ClinicUser> ClinicUsers => Set<ClinicUser>();

    // ===== DbSets - Negócio =====
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<ScheduleException> ScheduleExceptions => Set<ScheduleException>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
    public DbSet<MedicalRecordEntry> MedicalRecordEntries => Set<MedicalRecordEntry>();

    // ===== DbSets - Clínico =====
    public DbSet<ExamType> ExamTypes => Set<ExamType>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<ExamResult> ExamResults => Set<ExamResult>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
    public DbSet<Atest> Atests => Set<Atest>();

    // ===== DbSets - Convênios =====
    public DbSet<InsurancePlan> InsurancePlans => Set<InsurancePlan>();
    public DbSet<InsuranceCoverage> InsuranceCoverages => Set<InsuranceCoverage>();

    // ===== DbSets - Financeiro =====
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    // ===== DbSets - Suporte =====
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<WaitingRoomEntry> WaitingRoomEntries => Set<WaitingRoomEntry>();

    // ===== DbSets - Catálogo =====
    public DbSet<ProcedureType> ProcedureTypes => Set<ProcedureType>();

    public VitalGestDbContext(
        DbContextOptions<VitalGestDbContext> options,
        ITenantService tenantService)
        : base(options)
    {
        _tenantService = tenantService;
    }

    /// <summary>
    /// Configura o modelo: mapeamentos, relacionamentos, constraints e Global Query Filter.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== Aplica Global Query Filter para Multi-tenant =====
        // Filtra automaticamente por ClinicId em todas as queries
        // Ignora o filtro quando não há tenant (ex: endpoints públicos de auth)
        ApplyTenantFilter(modelBuilder);

        // ===== Configurações de Entidades =====
        ConfigureAddress(modelBuilder);
        ConfigureClinic(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigurePosition(modelBuilder);
        ConfigureDepartment(modelBuilder);
        ConfigureSpecialty(modelBuilder);
        ConfigureClinicUser(modelBuilder);
        ConfigurePatient(modelBuilder);
        ConfigureAppointment(modelBuilder);
        ConfigureSchedule(modelBuilder);
        ConfigureMedicalRecord(modelBuilder);
        ConfigureExam(modelBuilder);
        ConfigurePrescription(modelBuilder);
        ConfigureAtest(modelBuilder);
        ConfigureInsurance(modelBuilder);
        ConfigurePayment(modelBuilder);
        ConfigureInvoice(modelBuilder);
        ConfigureDocument(modelBuilder);
        ConfigureNotification(modelBuilder);
        ConfigureAuditLog(modelBuilder);
        ConfigureWaitingRoom(modelBuilder);
        ConfigureTimeSlot(modelBuilder);
        ConfigureScheduleException(modelBuilder);
        ConfigureProcedureType(modelBuilder);
    }

    /// <summary>
    /// Aplica o filtro multi-tenant em todas as entidades que possuem ClinicId.
    /// O filtro é ignorado quando ITenantService.ClinicId é null (ex: durante login/registro).
    /// </summary>
    private void ApplyTenantFilter(ModelBuilder modelBuilder)
    {
        // Lista de entidades que possuem ClinicId e devem ser filtradas
        // Entidades sem ClinicId não são filtradas (ex: Address, User, Position, Specialty, ExamType)

        // Multi-tenant entities
        modelBuilder.Entity<Department>().HasQueryFilter(e => _tenantService.ClinicId == null || e.ClinicId == _tenantService.ClinicId);
        modelBuilder.Entity<ClinicUser>().HasQueryFilter(e => _tenantService.ClinicId == null || e.ClinicId == _tenantService.ClinicId);
        modelBuilder.Entity<Patient>().HasQueryFilter(e => _tenantService.ClinicId == null || e.ClinicId == _tenantService.ClinicId);
        modelBuilder.Entity<Appointment>().HasQueryFilter(e => _tenantService.ClinicId == null || e.ClinicId == _tenantService.ClinicId);
        modelBuilder.Entity<Schedule>().HasQueryFilter(e => _tenantService.ClinicId == null || e.ClinicId == _tenantService.ClinicId);
        modelBuilder.Entity<ScheduleException>().HasQueryFilter(e => _tenantService.ClinicId == null || e.ClinicId == _tenantService.ClinicId);
        modelBuilder.Entity<TimeSlot>().HasQueryFilter(e => _tenantService.ClinicId == null || e.ClinicId == _tenantService.ClinicId);
        modelBuilder.Entity<MedicalRecord>().HasQueryFilter(e => _tenantService.ClinicId == null || e.ClinicId == _tenantService.ClinicId);
        modelBuilder.Entity<Exam>().HasQueryFilter(e => _tenantService.ClinicId == null || e.ClinicId == _tenantService.ClinicId);
        modelBuilder.Entity<Prescription>().HasQueryFilter(e => _tenantService.ClinicId == null || e.ClinicId == _tenantService.ClinicId);
        modelBuilder.Entity<Atest>().HasQueryFilter(e => _tenantService.ClinicId == null || e.ClinicId == _tenantService.ClinicId);
        modelBuilder.Entity<Payment>().HasQueryFilter(e => _tenantService.ClinicId == null || e.ClinicId == _tenantService.ClinicId);
        modelBuilder.Entity<Invoice>().HasQueryFilter(e => _tenantService.ClinicId == null || e.ClinicId == _tenantService.ClinicId);
        modelBuilder.Entity<Document>().HasQueryFilter(e => _tenantService.ClinicId == null || e.ClinicId == _tenantService.ClinicId);
        modelBuilder.Entity<Notification>().HasQueryFilter(e => _tenantService.ClinicId == null || e.ClinicId == _tenantService.ClinicId);
        modelBuilder.Entity<WaitingRoomEntry>().HasQueryFilter(e => _tenantService.ClinicId == null || e.ClinicId == _tenantService.ClinicId);
    }

    #region Entity Configurations (Fluent API)

    private static void ConfigureAddress(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("Addresses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Street).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Number).HasMaxLength(20);
            entity.Property(e => e.Complement).HasMaxLength(255);
            entity.Property(e => e.Neighborhood).HasMaxLength(255);
            entity.Property(e => e.City).IsRequired().HasMaxLength(255);
            entity.Property(e => e.State).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ZipCode).HasMaxLength(10);
            entity.Property(e => e.Country).HasMaxLength(100).HasDefaultValue("Brasil");
            entity.Property(e => e.Latitude).HasPrecision(10, 7);
            entity.Property(e => e.Longitude).HasPrecision(10, 7);
        });
    }

    private static void ConfigureClinic(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Clinic>(entity =>
        {
            entity.ToTable("Clinics");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CorporateName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CNPJ).IsRequired().HasMaxLength(18);
            entity.HasIndex(e => e.CNPJ).IsUnique();
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.SecondaryPhone).HasMaxLength(20);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Website).HasMaxLength(500);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.OpeningHours).HasColumnType("jsonb");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            // Relacionamentos
            entity.HasOne(e => e.Address)
                  .WithMany()
                  .HasForeignKey(e => e.AddressId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.Departments)
                  .WithOne(d => d.Clinic)
                  .HasForeignKey(d => d.ClinicId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.ClinicUsers)
                  .WithOne(cu => cu.Clinic)
                  .HasForeignKey(cu => cu.ClinicId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Patients)
                  .WithOne(p => p.Clinic)
                  .HasForeignKey(p => p.ClinicId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CPF).HasMaxLength(14);
            entity.HasIndex(e => e.CPF).IsUnique().HasFilter("\"CPF\" IS NOT NULL");
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
            entity.Property(e => e.RefreshToken).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasMany(e => e.ClinicUsers)
                  .WithOne(cu => cu.User)
                  .HasForeignKey(cu => cu.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigurePosition(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Position>(entity =>
        {
            entity.ToTable("Positions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });
    }

    private static void ConfigureDepartment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(e => e.Clinic)
                  .WithMany(c => c.Departments)
                  .HasForeignKey(e => e.ClinicId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ClinicId);
        });
    }

    private static void ConfigureSpecialty(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.ToTable("Specialties");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });
    }

    private static void ConfigureClinicUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClinicUser>(entity =>
        {
            entity.ToTable("ClinicUsers");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.ClinicId }).IsUnique();
            entity.Property(e => e.ProfessionalDocument).HasMaxLength(50);
            entity.Property(e => e.ProfessionalDocumentType).HasMaxLength(20);
            entity.Property(e => e.ProfessionalDocumentUF).HasMaxLength(2);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(e => e.User)
                  .WithMany(u => u.ClinicUsers)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Clinic)
                  .WithMany(c => c.ClinicUsers)
                  .HasForeignKey(e => e.ClinicId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Position)
                  .WithMany(p => p.ClinicUsers)
                  .HasForeignKey(e => e.PositionId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Department)
                  .WithMany(d => d.ClinicUsers)
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigurePatient(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("Patients");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CPF).HasMaxLength(14);
            entity.HasIndex(e => e.CPF).IsUnique().HasFilter("\"CPF\" IS NOT NULL");
            entity.Property(e => e.RG).HasMaxLength(20);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.SecondaryPhone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.InsuranceCardNumber).HasMaxLength(50);
            entity.Property(e => e.ProfilePhotoUrl).HasMaxLength(500);
            entity.Property(e => e.EmergencyContact).HasMaxLength(255);
            entity.Property(e => e.EmergencyPhone).HasMaxLength(20);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasIndex(e => e.ClinicId);
            entity.HasIndex(e => e.Phone);

            entity.HasOne(e => e.Clinic)
                  .WithMany(c => c.Patients)
                  .HasForeignKey(e => e.ClinicId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Address)
                  .WithMany()
                  .HasForeignKey(e => e.AddressId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.InsurancePlan)
                  .WithMany(ip => ip.Patients)
                  .HasForeignKey(e => e.InsurancePlanId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureAppointment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("Appointments");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.ClinicId);
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.DoctorUserId);
            entity.HasIndex(e => e.AppointmentDate).IsDescending();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.DoctorUserId, e.AppointmentDate });

            entity.HasOne(e => e.Patient)
                  .WithMany(p => p.Appointments)
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Doctor)
                  .WithMany(u => u.Appointments)
                  .HasForeignKey(e => e.DoctorUserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CreatedBy)
                  .WithMany(u => u.AppointmentsCreated)
                  .HasForeignKey(e => e.CreatedById)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Department)
                  .WithMany()
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Specialty)
                  .WithMany()
                  .HasForeignKey(e => e.SpecialtyId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureSchedule(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.ToTable("Schedules");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SlotDuration).HasDefaultValue(30);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasIndex(e => e.DoctorUserId);
            entity.HasIndex(e => e.ClinicId);
            entity.HasIndex(e => new { e.DoctorUserId, e.DayOfWeek });

            entity.HasOne(e => e.Doctor)
                  .WithMany(u => u.Schedules)
                  .HasForeignKey(e => e.DoctorUserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.TimeSlots)
                  .WithOne(ts => ts.Schedule)
                  .HasForeignKey(ts => ts.ScheduleId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureMedicalRecord(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.ToTable("MedicalRecords");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PatientId, e.ClinicId }).IsUnique();

            entity.HasOne(e => e.Patient)
                  .WithOne(p => p.MedicalRecord)
                  .HasForeignKey<MedicalRecord>(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Entries)
                  .WithOne(e => e.MedicalRecord)
                  .HasForeignKey(e => e.MedicalRecordId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MedicalRecordEntry>(entity =>
        {
            entity.ToTable("MedicalRecordEntries");

            entity.HasOne(e => e.Doctor)
                  .WithMany(u => u.MedicalRecordEntries)
                  .HasForeignKey(e => e.DoctorUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureExam(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExamType>(entity =>
        {
            entity.ToTable("ExamTypes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.ToTable("Exams");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.DoctorUserId);
            entity.HasIndex(e => e.Status);

            entity.HasOne(e => e.Patient)
                  .WithMany(p => p.Exams)
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Doctor)
                  .WithMany(u => u.Exams)
                  .HasForeignKey(e => e.DoctorUserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ExamType)
                  .WithMany(et => et.Exams)
                  .HasForeignKey(e => e.ExamTypeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Result)
                  .WithOne(r => r.Exam)
                  .HasForeignKey<ExamResult>(r => r.ExamId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ExamResult>(entity =>
        {
            entity.ToTable("ExamResults");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ResultJson).HasColumnType("jsonb");
            entity.Property(e => e.FileUrl).HasMaxLength(500);

            entity.HasOne(e => e.PerformedBy)
                  .WithMany(u => u.ExamResultsPerformed)
                  .HasForeignKey(e => e.PerformedById)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.ReviewedBy)
                  .WithMany(u => u.ExamResultsReviewed)
                  .HasForeignKey(e => e.ReviewedById)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigurePrescription(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.ToTable("Prescriptions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.DoctorUserId);

            entity.HasOne(e => e.Patient)
                  .WithMany(p => p.Prescriptions)
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Doctor)
                  .WithMany(u => u.Prescriptions)
                  .HasForeignKey(e => e.DoctorUserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Items)
                  .WithOne(i => i.Prescription)
                  .HasForeignKey(i => i.PrescriptionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PrescriptionItem>(entity =>
        {
            entity.ToTable("PrescriptionItems");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MedicationName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Dosage).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Frequency).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Duration).HasMaxLength(100);
            entity.Property(e => e.OrderNumber).HasDefaultValue(1);
        });
    }

    private static void ConfigureAtest(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Atest>(entity =>
        {
            entity.ToTable("Atests");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CID).HasMaxLength(10);
            entity.Property(e => e.Description).IsRequired();
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.DoctorUserId);

            entity.HasOne(e => e.Patient)
                  .WithMany(p => p.Atests)
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Doctor)
                  .WithMany(u => u.Atests)
                  .HasForeignKey(e => e.DoctorUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureInsurance(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InsurancePlan>(entity =>
        {
            entity.ToTable("InsurancePlans");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CNPJ).HasMaxLength(18);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasMany(e => e.Coverages)
                  .WithOne(c => c.InsurancePlan)
                  .HasForeignKey(c => c.InsurancePlanId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InsuranceCoverage>(entity =>
        {
            entity.ToTable("InsuranceCoverages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CoveragePercent).HasPrecision(5, 2).HasDefaultValue(100.00m);

            entity.HasOne(e => e.ExamType)
                  .WithMany(et => et.Coverages)
                  .HasForeignKey(e => e.ExamTypeId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Specialty)
                  .WithMany()
                  .HasForeignKey(e => e.SpecialtyId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigurePayment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(12, 2);
            entity.Property(e => e.Discount).HasPrecision(12, 2).HasDefaultValue(0);
            entity.Property(e => e.TotalAmount).HasPrecision(12, 2);
            entity.Property(e => e.Installments).HasDefaultValue(1);
            entity.HasIndex(e => e.ClinicId);
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.PaymentDate).IsDescending();

            entity.HasOne(e => e.Patient)
                  .WithMany(p => p.Payments)
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Appointment)
                  .WithMany(a => a.Payments)
                  .HasForeignKey(e => e.AppointmentId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.ReceivedBy)
                  .WithMany(u => u.PaymentsReceived)
                  .HasForeignKey(e => e.ReceivedById)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureInvoice(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("Invoices");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasPrecision(12, 2);
            entity.Property(e => e.PaidAmount).HasPrecision(12, 2).HasDefaultValue(0);
            entity.HasIndex(e => new { e.ClinicId, e.InvoiceNumber }).IsUnique();
            entity.HasIndex(e => e.ClinicId);
            entity.HasIndex(e => e.Status);

            entity.HasOne(e => e.Patient)
                  .WithMany()
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureDocument(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("Documents");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FileUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.HasIndex(e => e.ClinicId);
            entity.HasIndex(e => e.PatientId);

            entity.HasOne(e => e.Patient)
                  .WithMany(p => p.Documents)
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Appointment)
                  .WithMany()
                  .HasForeignKey(e => e.AppointmentId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.UploadedBy)
                  .WithMany(u => u.DocumentsUploaded)
                  .HasForeignKey(e => e.UploadedById)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureNotification(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notifications");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.Channel).HasMaxLength(20);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ClinicId);
            entity.HasIndex(e => e.IsRead).HasFilter("\"IsRead\" = FALSE");

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Notifications)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureAuditLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLogs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.Property(e => e.OldValues).HasColumnType("jsonb");
            entity.Property(e => e.NewValues).HasColumnType("jsonb");
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.HasIndex(e => e.ClinicId);
            entity.HasIndex(e => new { e.EntityType, e.EntityId });
            entity.HasIndex(e => e.CreatedAt).IsDescending();

            entity.HasOne(e => e.User)
                  .WithMany(u => u.AuditLogs)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureWaitingRoom(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WaitingRoomEntry>(entity =>
        {
            entity.ToTable("WaitingRoomEntries");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Priority).HasDefaultValue(1);
            entity.HasIndex(e => e.ClinicId);
            entity.HasIndex(e => e.Status);

            entity.HasOne(e => e.Appointment)
                  .WithMany()
                  .HasForeignKey(e => e.AppointmentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Patient)
                  .WithMany()
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureTimeSlot(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.ToTable("TimeSlots");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.DoctorUserId, e.Date });
            entity.HasIndex(e => e.IsAvailable).HasFilter("\"IsAvailable\" = TRUE");

            entity.HasOne(e => e.Doctor)
                  .WithMany(u => u.TimeSlots)
                  .HasForeignKey(e => e.DoctorUserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Schedule)
                  .WithMany(s => s.TimeSlots)
                  .HasForeignKey(e => e.ScheduleId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Appointment)
                  .WithOne(a => a.TimeSlot)
                  .HasForeignKey<TimeSlot>(e => e.AppointmentId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureScheduleException(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScheduleException>(entity =>
        {
            entity.ToTable("ScheduleExceptions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reason).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.DoctorUserId);
            entity.HasIndex(e => e.ExceptionDate);
            entity.HasIndex(e => new { e.DoctorUserId, e.ExceptionDate });

            entity.HasOne(e => e.Doctor)
                  .WithMany(u => u.ScheduleExceptions)
                  .HasForeignKey(e => e.DoctorUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureProcedureType(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcedureType>(entity =>
        {
            entity.ToTable("ProcedureTypes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description);
            entity.Property(e => e.TussCode).HasMaxLength(20);
            entity.Property(e => e.DefaultPrice).HasPrecision(12, 2);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.HasIndex(e => e.Name).HasMethod("gin").HasOperators("gin_trgm_ops");
            entity.HasIndex(e => e.Category);
        });
    }

    #endregion
}