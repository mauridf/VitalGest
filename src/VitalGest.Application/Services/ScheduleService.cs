using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Schedule;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Entities;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

/// <summary>
/// Serviço de gestão de agendas e horários dos profissionais.
/// </summary>
public class ScheduleService : IScheduleService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<ScheduleService> _logger;

    public ScheduleService(IUnitOfWork uow, IMapper mapper, ILogger<ScheduleService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ScheduleResponse>> GetByDoctorAsync(
        int clinicId,
        int doctorId,
        CancellationToken ct = default)
    {
        var schedules = await _uow.Schedules.GetByDoctorIdAsync(doctorId, clinicId, ct);
        return _mapper.Map<IEnumerable<ScheduleResponse>>(schedules);
    }

    /// <inheritdoc />
    public async Task<ScheduleResponse> CreateAsync(
        int clinicId,
        CreateScheduleRequest request,
        CancellationToken ct = default)
    {
        // Validações
        if (request.DayOfWeek < 0 || request.DayOfWeek > 6)
            throw new BusinessRuleException("Dia da semana inválido (0=Domingo, 6=Sábado).", "INVALID_DAY");

        if (request.StartTime >= request.EndTime)
            throw new BusinessRuleException("Horário inicial deve ser anterior ao horário final.", "INVALID_TIMES");

        if (request.SlotDuration < 15 || request.SlotDuration > 120)
            throw new BusinessRuleException("Duração do slot deve ser entre 15 e 120 minutos.", "INVALID_SLOT_DURATION");

        var schedule = _mapper.Map<Schedule>(request);
        schedule.ClinicId = clinicId;
        schedule.CreatedAt = DateTime.UtcNow;

        await _uow.Schedules.AddAsync(schedule, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Regra de agenda criada: Doctor {DoctorId}, Day {Day}",
            request.DoctorUserId, request.DayOfWeek);

        return _mapper.Map<ScheduleResponse>(schedule);
    }

    /// <inheritdoc />
    public async Task<ScheduleResponse> UpdateAsync(
        int id,
        int clinicId,
        UpdateScheduleRequest request,
        CancellationToken ct = default)
    {
        var schedule = await _uow.Schedules.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Regra de agenda", id);

        if (schedule.ClinicId != clinicId)
            throw new BusinessRuleException("Regra não pertence a esta clínica.", "WRONG_CLINIC");

        if (request.StartTime >= request.EndTime)
            throw new BusinessRuleException("Horário inicial deve ser anterior ao horário final.", "INVALID_TIMES");

        schedule.StartTime = request.StartTime;
        schedule.EndTime = request.EndTime;
        schedule.SlotDuration = request.SlotDuration;
        schedule.IsActive = request.IsActive;
        schedule.UpdatedAt = DateTime.UtcNow;

        await _uow.Schedules.UpdateAsync(schedule, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<ScheduleResponse>(schedule);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, int clinicId, CancellationToken ct = default)
    {
        var schedule = await _uow.Schedules.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Regra de agenda", id);

        if (schedule.ClinicId != clinicId)
            throw new BusinessRuleException("Regra não pertence a esta clínica.", "WRONG_CLINIC");

        await _uow.Schedules.DeleteAsync(schedule, ct);
        await _uow.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task<ScheduleExceptionResponse> CreateExceptionAsync(
        int clinicId,
        CreateScheduleExceptionRequest request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
            throw new BusinessRuleException("Motivo da exceção é obrigatório.", "REASON_REQUIRED");

        var exception = new ScheduleException
        {
            ClinicId = clinicId,
            DoctorUserId = request.DoctorUserId,
            ExceptionDate = request.ExceptionDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Reason = request.Reason,
            IsAvailable = request.IsAvailable,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.ScheduleExceptions.AddAsync(exception, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<ScheduleExceptionResponse>(exception);
    }

    /// <inheritdoc />
    public async Task DeleteExceptionAsync(int id, int clinicId, CancellationToken ct = default)
    {
        var exception = await _uow.ScheduleExceptions.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Exceção de agenda", id);

        if (exception.ClinicId != clinicId)
            throw new BusinessRuleException("Exceção não pertence a esta clínica.", "WRONG_CLINIC");

        await _uow.ScheduleExceptions.DeleteAsync(exception, ct);
        await _uow.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TimeSlotResponse>> GetAvailableSlotsAsync(
        int clinicId,
        int doctorId,
        DateOnly date,
        CancellationToken ct = default)
    {
        var slots = await _uow.Schedules.GetAvailableSlotsAsync(doctorId, date, clinicId, ct);
        return _mapper.Map<IEnumerable<TimeSlotResponse>>(slots);
    }
}