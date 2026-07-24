using VitalGest.Application.DTOs.Appointments;
using VitalGest.Core.Enums;

namespace VitalGest.Application.Interfaces;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentResponse>> GetByDateAsync(int clinicId, DateOnly date, CancellationToken ct = default);
    Task<AppointmentResponse> GetByIdAsync(int id, int clinicId, CancellationToken ct = default);
    Task<AppointmentResponse> CreateAsync(int clinicId, int createdById, CreateAppointmentRequest request, CancellationToken ct = default);
    Task<AppointmentResponse> UpdateAsync(int id, int clinicId, UpdateAppointmentRequest request, CancellationToken ct = default);
    Task<AppointmentResponse> UpdateStatusAsync(int id, int clinicId, UpdateAppointmentStatusRequest request, CancellationToken ct = default);
    Task<AppointmentResponse> ConfirmAsync(int id, int clinicId, CancellationToken ct = default);
    Task<AppointmentResponse> CancelAsync(int id, int clinicId, string reason, CancellationToken ct = default);
    Task<AppointmentResponse> MarkNoShowAsync(int id, int clinicId, CancellationToken ct = default);
    Task<IEnumerable<AppointmentResponse>> GetByDoctorAsync(int clinicId, int doctorId, CancellationToken ct = default);
    Task<IEnumerable<AppointmentResponse>> GetByPatientAsync(int patientId, int clinicId, CancellationToken ct = default);
}