using VitalGest.Application.DTOs.Atests;

namespace VitalGest.Application.Interfaces;

public interface IAtestService
{
    Task<IEnumerable<AtestResponse>> GetByPatientAsync(int patientId, int clinicId, CancellationToken ct = default);
    Task<AtestResponse> GetByIdAsync(int id, int clinicId, CancellationToken ct = default);
    Task<AtestResponse> CreateAsync(int clinicId, int doctorUserId, CreateAtestRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, int clinicId, int doctorUserId, CancellationToken ct = default);
}