using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Application.DTOs.Audit;
using VitalGest.Application.DTOs.Common;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de log de auditoria.
/// Acesso restrito a Admin.
/// </summary>
[Authorize(Policy = "AdminOnly")]
public class AuditController : BaseApiController
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    /// <summary>
    /// Lista logs de auditoria da clínica com paginação.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<AuditLogResponse>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _auditService.GetAllAsync(clinicId, request);
        return OkPagedResponse(result);
    }

    /// <summary>
    /// Consulta auditoria por entidade específica.
    /// </summary>
    [HttpGet("entity/{entityType}/{entityId:int}")]
    [ProducesResponseType(typeof(IEnumerable<AuditLogResponse>), 200)]
    public async Task<IActionResult> GetByEntity(string entityType, int entityId)
    {
        var result = await _auditService.GetByEntityAsync(entityType, entityId);
        return OkResponse(result);
    }

    /// <summary>
    /// Consulta auditoria por usuário.
    /// </summary>
    [HttpGet("user/{userId:int}")]
    [ProducesResponseType(typeof(IEnumerable<AuditLogResponse>), 200)]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var result = await _auditService.GetByUserAsync(userId);
        return OkResponse(result);
    }
}