using AutoMapper;
using VitalGest.Application.DTOs.Audit;
using VitalGest.Application.DTOs.Common;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

public class AuditService : IAuditService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AuditService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<PagedResponse<AuditLogResponse>> GetAllAsync(int clinicId, PagedRequest request, CancellationToken ct = default)
    {
        var logs = await _uow.AuditLogs.GetPagedAsync(request.Page, request.PageSize, l => l.ClinicId == clinicId, ct);
        var count = await _uow.AuditLogs.CountAsync(l => l.ClinicId == clinicId, ct);
        return PagedResponse.Create(_mapper.Map<IEnumerable<AuditLogResponse>>(logs), request.Page, request.PageSize, count);
    }

    public async Task<IEnumerable<AuditLogResponse>> GetByEntityAsync(string entityType, int entityId, CancellationToken ct = default)
    {
        var logs = await _uow.AuditLogs.FindAsync(l => l.EntityType == entityType && l.EntityId == entityId, ct);
        return _mapper.Map<IEnumerable<AuditLogResponse>>(logs);
    }

    public async Task<IEnumerable<AuditLogResponse>> GetByUserAsync(int userId, CancellationToken ct = default)
    {
        var logs = await _uow.AuditLogs.FindAsync(l => l.UserId == userId, ct);
        return _mapper.Map<IEnumerable<AuditLogResponse>>(logs);
    }
}