using VitalGest.Core.Entities;
using VitalGest.Core.Enums;

namespace VitalGest.Core.Interfaces;

/// <summary>
/// Repositório especializado para Agendamentos.
/// </summary>
public interface IAppointmentRepository : IRepository<Appointment>
{
    /// <summary>Busca agendamentos por data específica</summary>
    Task<IEnumerable<Appointment>> GetByDateAsync(DateOnly date, int clinicId, CancellationToken cancellationToken = default);

    /// <summary>Busca agendamentos por médico em um período</summary>
    Task<IEnumerable<Appointment>> GetByDoctorAndDateRangeAsync(
        int doctorUserId,
        DateOnly startDate,
        DateOnly endDate,
        int clinicId,
        CancellationToken cancellationToken = default);

    /// <summary>Busca agendamentos por paciente</summary>
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId, int clinicId, CancellationToken cancellationToken = default);

    /// <summary>Verifica conflito de horário para médico</summary>
    Task<bool> HasTimeConflictAsync(
        int doctorUserId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        int? excludeAppointmentId = null,
        CancellationToken cancellationToken = default);

    /// <summary>Busca agendamentos com detalhes (paciente, médico, especialidade)</summary>
    Task<Appointment?> GetByIdWithDetailsAsync(int appointmentId, int clinicId, CancellationToken cancellationToken = default);

    /// <summary>Lista agendamentos por status</summary>
    Task<IEnumerable<Appointment>> GetByStatusAsync(
        AppointmentStatus status,
        int clinicId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>Conta agendamentos do dia para dashboard</summary>
    Task<int> CountTodayAsync(int clinicId, CancellationToken cancellationToken = default);
}