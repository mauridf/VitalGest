using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Application.DTOs.Search;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de busca global.
/// </summary>
[Authorize]
public class SearchController : BaseApiController
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    /// <summary>
    /// Busca global em pacientes, agendamentos e médicos.
    /// Mínimo de 2 caracteres.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(SearchResponse), 200)]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] string? type = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");

        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return BadRequest(new { Success = false, Message = "A busca deve ter no mínimo 2 caracteres." });

        var request = new SearchRequest(query, type, page, pageSize);
        var result = await _searchService.SearchAsync(clinicId, request);
        return OkResponse(result);
    }
}