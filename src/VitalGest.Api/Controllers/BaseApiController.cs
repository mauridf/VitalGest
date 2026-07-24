using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller base para todos os controllers da API.
/// Define rota padrão, atributos comuns e helpers.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Obtém o ID do usuário logado a partir do token JWT.
    /// </summary>
    protected int GetUserId()
    {
        var subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirst("sub");

        if (subClaim == null || !int.TryParse(subClaim.Value, out var userId))
            throw new UnauthorizedAccessException("Usuário não autenticado.");

        return userId;
    }

    /// <summary>
    /// Obtém o ClinicId do token JWT (para operações multi-tenant).
    /// </summary>
    protected int? GetClinicId()
    {
        var clinicIdClaim = User.FindFirst("clinic_id");
        if (clinicIdClaim != null && int.TryParse(clinicIdClaim.Value, out var clinicId))
            return clinicId;

        return null;
    }

    /// <summary>
    /// Obtém a role do usuário logado.
    /// </summary>
    protected string? GetUserRole()
    {
        return User.FindFirst("role")?.Value;
    }

    /// <summary>
    /// Retorna resposta padronizada de sucesso.
    /// </summary>
    protected IActionResult OkResponse<T>(T data, string? message = null)
    {
        return Ok(new
        {
            Success = true,
            Message = message ?? "Operação realizada com sucesso.",
            Data = data
        });
    }

    /// <summary>
    /// Retorna resposta paginada padronizada.
    /// </summary>
    protected IActionResult OkPagedResponse<T>(Application.DTOs.Common.PagedResponse<T> pagedData)
    {
        return Ok(new
        {
            Success = true,
            Data = pagedData.Items,
            Pagination = new
            {
                pagedData.Page,
                pagedData.PageSize,
                pagedData.TotalCount,
                pagedData.TotalPages,
                pagedData.HasNextPage,
                pagedData.HasPreviousPage
            }
        });
    }
}