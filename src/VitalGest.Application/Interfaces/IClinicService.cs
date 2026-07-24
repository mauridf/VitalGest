using VitalGest.Application.DTOs.Clinics;
using VitalGest.Application.DTOs.Common;

namespace VitalGest.Application.Interfaces;

public interface IClinicService
{
    Task<ClinicResponse> CreateAsync(CreateClinicRequest request, CancellationToken ct = default);
    Task<ClinicResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ClinicResponse> UpdateAsync(int id, UpdateClinicRequest request, CancellationToken ct = default);
    Task<ClinicStatsResponse> GetStatsAsync(int clinicId, CancellationToken ct = default);
    Task<DepartmentResponse> CreateDepartmentAsync(int clinicId, CreateDepartmentRequest request, CancellationToken ct = default);
    Task<IEnumerable<DepartmentResponse>> GetDepartmentsAsync(int clinicId, CancellationToken ct = default);
}