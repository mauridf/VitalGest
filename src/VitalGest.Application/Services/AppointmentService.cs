using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Appointments;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

/// <summary>
/// Serviço de gestão de agendamentos.
/// </summary>
public class AppointmentService : IAppointmentService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(IUnitOfWork uow, IMapper mapper, ILogger<AppointmentService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AppointmentResponse>> GetByDateAsync(
        int clinicId,
        DateOnly date,
        CancellationToken ct = default)
    {
        var appointments = await _uow.Appointments.GetByDateAsync(date, clinicId, ct);
        return _mapper.Map<IEnumerable<AppointmentResponse>>(appointments);
    }

    /// <inheritdoc />
    public async Task<AppointmentResponse> GetByIdAsync(int id, int clinicId, CancellationToken ct = default)
    {
        var appointment = await _uow.Appointments.GetByIdWithDetailsAsync(id, clinicId, ct)
            ?? throw new NotFoundException("Agendamento", id);

        return _mapper.Map<AppointmentResponse>(appointment);
    }

    /// <inheritdoc />
    public async Task<AppointmentResponse> CreateAsync(
        int clinicId,
        int createdById,
        CreateAppointmentRequest request,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Criando agendamento: Paciente {PatientId}, Médico {DoctorId}, Data {Date}",
            request.PatientId, request.DoctorUserId, request.AppointmentDate);

        // Verifica se a data é futura
        if (request.AppointmentDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new BusinessRuleException("Não é possível agendar para uma data passada.", "INVALID_DATE");

        // Verifica conflito de horário do médico
        var hasConflict = await _uow.Appointments.HasTimeConflictAsync(
            request.DoctorUserId,
            request.AppointmentDate,
            request.StartTime,
            request.EndTime,
            cancellationToken: ct);

        if (hasConflict)
            throw new BusinessRuleException("O médico já possui um agendamento neste horário.", "TIME_CONFLICT");

        // Verifica se paciente existe e pertence à clínica
        var patient = await _uow.Patients.GetByIdAsync(request.PatientId, ct);
        if (patient == null || patient.ClinicId != clinicId)
            throw new NotFoundException("Paciente", request.PatientId);

        // Cria agendamento
        var appointment = _mapper.Map<Appointment>(request);
        appointment.ClinicId = clinicId;
        appointment.CreatedById = createdById;
        appointment.Status = AppointmentStatus.Scheduled;
        appointment.CreatedAt = DateTime.UtcNow;

        await _uow.Appointments.AddAsync(appointment, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Agendamento criado: {AppointmentId}", appointment.Id);

        return _mapper.Map<AppointmentResponse>(appointment);
    }

    /// <inheritdoc />
    public async Task<AppointmentResponse> UpdateAsync(
        int id,
        int clinicId,
        UpdateAppointmentRequest request,
        CancellationToken ct = default)
    {
        var appointment = await _uow.Appointments.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Agendamento", id);

        if (appointment.ClinicId != clinicId)
            throw new BusinessRuleException("Agendamento não pertence a esta clínica.", "WRONG_CLINIC");

        // Só permite alterar agendamentos futuros
        if (appointment.AppointmentDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new BusinessRuleException("Não é possível alterar agendamentos passados.", "PAST_APPOINTMENT");

        // Verifica conflito (excluindo o próprio agendamento)
        var hasConflict = await _uow.Appointments.HasTimeConflictAsync(
            appointment.DoctorUserId,
            request.AppointmentDate,
            request.StartTime,
            request.EndTime,
            excludeAppointmentId: id,
            cancellationToken: ct);

        if (hasConflict)
            throw new BusinessRuleException("O médico já possui um agendamento neste horário.", "TIME_CONFLICT");

        appointment.AppointmentDate = request.AppointmentDate;
        appointment.StartTime = request.StartTime;
        appointment.EndTime = request.EndTime;
        appointment.Type = request.Type;
        appointment.DepartmentId = request.DepartmentId;
        appointment.SpecialtyId = request.SpecialtyId;
        appointment.Notes = request.Notes;
        appointment.InternalNotes = request.InternalNotes;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _uow.Appointments.UpdateAsync(appointment, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<AppointmentResponse>(appointment);
    }

    /// <inheritdoc />
    public async Task<AppointmentResponse> UpdateStatusAsync(
        int id,
        int clinicId,
        UpdateAppointmentStatusRequest request,
        CancellationToken ct = default)
    {
        var appointment = await _uow.Appointments.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Agendamento", id);

        if (appointment.ClinicId != clinicId)
            throw new BusinessRuleException("Agendamento não pertence a esta clínica.", "WRONG_CLINIC");

        // Regras de transição de status
        ValidateStatusTransition(appointment.Status, request.Status);

        appointment.Status = request.Status;

        if (request.Status == AppointmentStatus.Cancelled)
        {
            appointment.CancelledAt = DateTime.UtcNow;
            appointment.CancelReason = request.CancelReason;
        }

        appointment.UpdatedAt = DateTime.UtcNow;

        await _uow.Appointments.UpdateAsync(appointment, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<AppointmentResponse>(appointment);
    }

    /// <inheritdoc />
    public async Task<AppointmentResponse> ConfirmAsync(int id, int clinicId, CancellationToken ct = default)
    {
        var appointment = await _uow.Appointments.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Agendamento", id);

        if (appointment.Status != AppointmentStatus.Scheduled)
            throw new BusinessRuleException("Apenas agendamentos com status 'Agendado' podem ser confirmados.", "INVALID_STATUS");

        appointment.Status = AppointmentStatus.Confirmed;
        appointment.IsConfirmed = true;
        appointment.ConfirmedAt = DateTime.UtcNow;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _uow.Appointments.UpdateAsync(appointment, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<AppointmentResponse>(appointment);
    }

    /// <inheritdoc />
    public async Task<AppointmentResponse> CancelAsync(int id, int clinicId, string reason, CancellationToken ct = default)
    {
        var appointment = await _uow.Appointments.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Agendamento", id);

        // Só permite cancelar agendamentos futuros
        if (appointment.AppointmentDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new BusinessRuleException("Não é possível cancelar agendamentos passados.", "PAST_APPOINTMENT");

        if (appointment.Status == AppointmentStatus.Cancelled)
            throw new BusinessRuleException("Agendamento já está cancelado.", "ALREADY_CANCELLED");

        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessRuleException("Motivo do cancelamento é obrigatório.", "CANCEL_REASON_REQUIRED");

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.CancelledAt = DateTime.UtcNow;
        appointment.CancelReason = reason;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _uow.Appointments.UpdateAsync(appointment, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Agendamento cancelado: {AppointmentId}, Motivo: {Reason}", id, reason);

        return _mapper.Map<AppointmentResponse>(appointment);
    }

    /// <inheritdoc />
    public async Task<AppointmentResponse> MarkNoShowAsync(int id, int clinicId, CancellationToken ct = default)
    {
        var appointment = await _uow.Appointments.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Agendamento", id);

        if (appointment.Status != AppointmentStatus.Scheduled && appointment.Status != AppointmentStatus.Confirmed)
            throw new BusinessRuleException("Status inválido para marcar como não compareceu.", "INVALID_STATUS");

        appointment.Status = AppointmentStatus.NoShow;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _uow.Appointments.UpdateAsync(appointment, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<AppointmentResponse>(appointment);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AppointmentResponse>> GetByPatientAsync(int patientId, int clinicId, CancellationToken ct = default)
    {
        var appointments = await _uow.Appointments.GetByPatientIdAsync(patientId, clinicId, ct);
        return _mapper.Map<IEnumerable<AppointmentResponse>>(appointments);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AppointmentResponse>> GetByDoctorAsync(
        int clinicId,
        int doctorId,
        CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = today.AddDays(30);

        var appointments = await _uow.Appointments.GetByDoctorAndDateRangeAsync(
            doctorId, today, endDate, clinicId, ct);

        return _mapper.Map<IEnumerable<AppointmentResponse>>(appointments);
    }

    /// <summary>
    /// Valida as transições permitidas entre status de agendamento.
    /// </summary>
    private static void ValidateStatusTransition(AppointmentStatus current, AppointmentStatus next)
    {
        var validTransitions = new Dictionary<AppointmentStatus, AppointmentStatus[]>
        {
            { AppointmentStatus.Scheduled, [AppointmentStatus.Confirmed, AppointmentStatus.Cancelled, AppointmentStatus.InProgress] },
            { AppointmentStatus.Confirmed, [AppointmentStatus.InProgress, AppointmentStatus.Cancelled, AppointmentStatus.NoShow] },
            { AppointmentStatus.InProgress, [AppointmentStatus.Completed] },
            { AppointmentStatus.Completed, [] },
            { AppointmentStatus.Cancelled, [] },
            { AppointmentStatus.NoShow, [] }
        };

        if (validTransitions.TryGetValue(current, out var allowed) && !allowed.Contains(next))
            throw new BusinessRuleException(
                $"Não é permitido transitar de '{current}' para '{next}'.",
                "INVALID_STATUS_TRANSITION");
    }
}