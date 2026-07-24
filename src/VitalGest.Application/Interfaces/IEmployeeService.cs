using VitalGest.Application.DTOs.Common;
using VitalGest.Application.DTOs.Employees;

namespace VitalGest.Application.Interfaces;

public interface IEmployeeService
{
    Task<PagedResponse<EmployeeResponse>> GetAllAsync(int clinicId, PagedRequest request, CancellationToken ct = default);
    Task<EmployeeResponse> GetByIdAsync(int id, int clinicId, CancellationToken ct = default);
    Task<EmployeeResponse> CreateAsync(int clinicId, CreateEmployeeRequest request, CancellationToken ct = default);
    Task<EmployeeResponse> UpdateAsync(int id, int clinicId, UpdateEmployeeRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, int clinicId, CancellationToken ct = default);
}
