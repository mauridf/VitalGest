using VitalGest.Application.DTOs.Insurance;

namespace VitalGest.Application.Interfaces;

public interface IInsuranceService
{
    Task<IEnumerable<InsurancePlanResponse>> GetAllAsync(CancellationToken ct = default);
    Task<InsurancePlanResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task<InsurancePlanResponse> CreateAsync(CreateInsurancePlanRequest request, CancellationToken ct = default);
    Task<InsurancePlanResponse> UpdateAsync(int id, UpdateInsurancePlanRequest request, CancellationToken ct = default);
    Task<IEnumerable<InsuranceCoverageResponse>> GetCoveragesAsync(int planId, CancellationToken ct = default);
    Task<InsuranceCoverageResponse> AddCoverageAsync(int planId, CreateInsuranceCoverageRequest request, CancellationToken ct = default);
}