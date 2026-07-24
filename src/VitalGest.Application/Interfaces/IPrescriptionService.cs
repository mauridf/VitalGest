using VitalGest.Application.DTOs.Prescriptions;

namespace VitalGest.Application.Interfaces;

public interface IPrescriptionService
{
    Task<IEnumerable<PrescriptionResponse>> GetByPatientAsync(int patientId, int clinicId, CancellationToken ct = default);
    Task<PrescriptionResponse> GetByIdAsync(int id, int clinicId, CancellationToken ct = default);
    Task<PrescriptionResponse> CreateAsync(int clinicId, int doctorUserId, CreatePrescriptionRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, int clinicId, int doctorUserId, CancellationToken ct = default);
    Task<PrescriptionResponse> UpdateAsync(int id, int clinicId, CreatePrescriptionRequest request, CancellationToken ct = default);
}