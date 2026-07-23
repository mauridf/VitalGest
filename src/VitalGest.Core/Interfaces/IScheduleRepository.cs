using VitalGest.Core.Entities;

namespace VitalGest.Core.Interfaces;

/// <summary>
/// Repositório especializado para Agendas.
/// </summary>
public interface IScheduleRepository : IRepository<Schedule>
{
    /// <summary>Busca regras de agenda do médico</summary>
    Task<IEnumerable<Schedule>> GetByDoctorIdAsync(int doctorUserId, int clinicId, CancellationToken cancellationToken = default);

    /// <summary>Busca exceções de agenda do médico em um período</summary>
    Task<IEnumerable<ScheduleException>> GetExceptionsAsync(
        int doctorUserId,
        DateOnly startDate,
        DateOnly endDate,
        int clinicId,
        CancellationToken cancellationToken = default);

    /// <summary>Busca slots disponíveis para médico/data</summary>
    Task<IEnumerable<TimeSlot>> GetAvailableSlotsAsync(
        int doctorUserId,
        DateOnly date,
        int clinicId,
        CancellationToken cancellationToken = default);

    /// <summary>Gera slots para um período</summary>
    Task GenerateSlotsAsync(
        int doctorUserId,
        DateOnly startDate,
        DateOnly endDate,
        int clinicId,
        CancellationToken cancellationToken = default);

    /// <summary>Reserva um slot (vincula a appointment)</summary>
    Task ReserveSlotAsync(int slotId, int appointmentId, CancellationToken cancellationToken = default);

    /// <summary>Libera um slot</summary>
    Task ReleaseSlotAsync(int slotId, CancellationToken cancellationToken = default);
}