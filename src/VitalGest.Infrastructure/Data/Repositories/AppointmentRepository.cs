using Microsoft.EntityFrameworkCore;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;
using VitalGest.Core.Interfaces;
using VitalGest.Infrastructure.Data.Context;

namespace VitalGest.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório especializado para Agendamentos.
/// Implementa verificações de conflito e consultas por data/médico/status.
/// </summary>
public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(VitalGestDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<IEnumerable<Appointment>> GetByDateAsync(
        DateOnly date,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Include(a => a.Specialty)
            .Where(a => a.ClinicId == clinicId && a.AppointmentDate == date)
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .OrderBy(a => a.StartTime)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Appointment>> GetByDoctorAndDateRangeAsync(
        int doctorUserId,
        DateOnly startDate,
        DateOnly endDate,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Where(a => a.ClinicId == clinicId
                && a.DoctorUserId == doctorUserId
                && a.AppointmentDate >= startDate
                && a.AppointmentDate <= endDate)
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(
        int patientId,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Doctor)
            .Include(a => a.Specialty)
            .Where(a => a.ClinicId == clinicId && a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ThenByDescending(a => a.StartTime)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> HasTimeConflictAsync(
        int doctorUserId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        int? excludeAppointmentId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(a =>
            a.DoctorUserId == doctorUserId
            && a.AppointmentDate == date
            && a.Status != AppointmentStatus.Cancelled
            && a.Status != AppointmentStatus.NoShow
            // Verifica sobreposição de horários
            && a.StartTime < endTime
            && a.EndTime > startTime);

        // Exclui o próprio agendamento (para updates)
        if (excludeAppointmentId.HasValue)
            query = query.Where(a => a.Id != excludeAppointmentId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Appointment?> GetByIdWithDetailsAsync(
        int appointmentId,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Include(a => a.Specialty)
            .Include(a => a.Department)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.ClinicId == clinicId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Appointment>> GetByStatusAsync(
        AppointmentStatus status,
        int clinicId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.ClinicId == clinicId && a.Status == status)
            .OrderByDescending(a => a.AppointmentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountTodayAsync(int clinicId, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await _dbSet
            .Where(a => a.ClinicId == clinicId
                && a.AppointmentDate == today
                && a.Status != AppointmentStatus.Cancelled)
            .CountAsync(cancellationToken);
    }
}