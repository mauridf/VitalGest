using VitalGest.Application.DTOs.Patients;
using VitalGest.Application.DTOs.Common;

namespace VitalGest.Application.Interfaces;

public interface IPatientService
{
    Task<PagedResponse<PatientListResponse>> GetAllAsync(int clinicId, PagedRequest request, CancellationToken ct = default);
    Task<PatientResponse> GetByIdAsync(int id, int clinicId, CancellationToken ct = default);
    Task<PatientResponse> CreateAsync(int clinicId, CreatePatientRequest request, CancellationToken ct = default);
    Task<PatientResponse> UpdateAsync(int id, int clinicId, UpdatePatientRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, int clinicId, CancellationToken ct = default);
    Task<PatientHistoryResponse> GetHistoryAsync(int id, int clinicId, CancellationToken ct = default);
    Task<PagedResponse<PatientListResponse>> SearchAsync(int clinicId, string query, PagedRequest request, CancellationToken ct = default);
}