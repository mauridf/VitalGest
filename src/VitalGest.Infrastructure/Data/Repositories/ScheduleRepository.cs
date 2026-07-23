using Microsoft.EntityFrameworkCore;
using VitalGest.Core.Entities;
using VitalGest.Core.Interfaces;
using VitalGest.Infrastructure.Data.Context;

namespace VitalGest.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório especializado para Agendas e Slots.
/// </summary>
public class ScheduleRepository : Repository<Schedule>, IScheduleRepository
{
    public ScheduleRepository(VitalGestDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<IEnumerable<Schedule>> GetByDoctorIdAsync(
        int doctorUserId,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.DoctorUserId == doctorUserId && s.ClinicId == clinicId && s.IsActive)
            .OrderBy(s => s.DayOfWeek)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ScheduleException>> GetExceptionsAsync(
        int doctorUserId,
        DateOnly startDate,
        DateOnly endDate,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleExceptions
            .Where(e => e.DoctorUserId == doctorUserId
                && e.ClinicId == clinicId
                && e.ExceptionDate >= startDate
                && e.ExceptionDate <= endDate)
            .OrderBy(e => e.ExceptionDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TimeSlot>> GetAvailableSlotsAsync(
        int doctorUserId,
        DateOnly date,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TimeSlots
            .Where(ts => ts.DoctorUserId == doctorUserId
                && ts.ClinicId == clinicId
                && ts.Date == date
                && ts.IsAvailable)
            .OrderBy(ts => ts.StartTime)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task GenerateSlotsAsync(
        int doctorUserId,
        DateOnly startDate,
        DateOnly endDate,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        // Busca regras de agenda do médico
        var schedules = await GetByDoctorIdAsync(doctorUserId, clinicId, cancellationToken);
        var scheduleList = schedules.ToList();

        if (scheduleList.Count == 0)
            return;

        // Busca exceções no período
        var exceptions = await GetExceptionsAsync(doctorUserId, startDate, endDate, clinicId, cancellationToken);
        var exceptionDates = exceptions
            .Where(e => !e.IsAvailable)
            .Select(e => e.ExceptionDate)
            .ToHashSet();

        // Remove slots existentes no período para regenerar
        var existingSlots = await _context.TimeSlots
            .Where(ts => ts.DoctorUserId == doctorUserId
                && ts.ClinicId == clinicId
                && ts.Date >= startDate
                && ts.Date <= endDate
                && ts.IsAvailable)
            .ToListAsync(cancellationToken);

        _context.TimeSlots.RemoveRange(existingSlots);

        // Gera slots para cada dia no período
        var currentDate = startDate;
        while (currentDate <= endDate)
        {
            // Pula datas com exceção
            if (exceptionDates.Contains(currentDate))
            {
                currentDate = currentDate.AddDays(1);
                continue;
            }

            // Encontra regra para o dia da semana
            var dayOfWeek = (int)currentDate.DayOfWeek;
            var daySchedule = scheduleList.FirstOrDefault(s => s.DayOfWeek == dayOfWeek);

            if (daySchedule != null)
            {
                // Gera slots com base na duração configurada
                var slotStart = daySchedule.StartTime;
                var slotEnd = slotStart.AddMinutes(daySchedule.SlotDuration);

                while (slotEnd <= daySchedule.EndTime)
                {
                    var slot = new TimeSlot
                    {
                        ClinicId = clinicId,
                        ScheduleId = daySchedule.Id,
                        DoctorUserId = doctorUserId,
                        Date = currentDate,
                        StartTime = slotStart,
                        EndTime = slotEnd,
                        IsAvailable = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _context.TimeSlots.AddAsync(slot, cancellationToken);

                    slotStart = slotEnd;
                    slotEnd = slotStart.AddMinutes(daySchedule.SlotDuration);
                }
            }

            currentDate = currentDate.AddDays(1);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task ReserveSlotAsync(
        int slotId,
        int appointmentId,
        CancellationToken cancellationToken = default)
    {
        var slot = await _context.TimeSlots.FindAsync([slotId], cancellationToken);
        if (slot != null && slot.IsAvailable)
        {
            slot.IsAvailable = false;
            slot.AppointmentId = appointmentId;
            _context.TimeSlots.Update(slot);
        }
    }

    /// <inheritdoc />
    public async Task ReleaseSlotAsync(int slotId, CancellationToken cancellationToken = default)
    {
        var slot = await _context.TimeSlots.FindAsync([slotId], cancellationToken);
        if (slot != null)
        {
            slot.IsAvailable = true;
            slot.AppointmentId = null;
            _context.TimeSlots.Update(slot);
        }
    }
}