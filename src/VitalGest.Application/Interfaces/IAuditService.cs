using VitalGest.Application.DTOs.Audit;
using VitalGest.Application.DTOs.Common;

namespace VitalGest.Application.Interfaces;

public interface IAuditService
{
    Task<PagedResponse<AuditLogResponse>> GetAllAsync(int clinicId, PagedRequest request, CancellationToken ct = default);
    Task<IEnumerable<AuditLogResponse>> GetByEntityAsync(string entityType, int entityId, CancellationToken ct = default);
    Task<IEnumerable<AuditLogResponse>> GetByUserAsync(int userId, CancellationToken ct = default);
}