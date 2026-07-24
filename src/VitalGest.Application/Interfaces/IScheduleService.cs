using VitalGest.Application.DTOs.Schedule;

namespace VitalGest.Application.Interfaces;

public interface IScheduleService
{
    Task<IEnumerable<ScheduleResponse>> GetByDoctorAsync(int clinicId, int doctorId, CancellationToken ct = default);
    Task<ScheduleResponse> CreateAsync(int clinicId, CreateScheduleRequest request, CancellationToken ct = default);
    Task<ScheduleResponse> UpdateAsync(int id, int clinicId, UpdateScheduleRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, int clinicId, CancellationToken ct = default);
    Task<ScheduleExceptionResponse> CreateExceptionAsync(int clinicId, CreateScheduleExceptionRequest request, CancellationToken ct = default);
    Task DeleteExceptionAsync(int id, int clinicId, CancellationToken ct = default);
    Task<IEnumerable<TimeSlotResponse>> GetAvailableSlotsAsync(int clinicId, int doctorId, DateOnly date, CancellationToken ct = default);
}