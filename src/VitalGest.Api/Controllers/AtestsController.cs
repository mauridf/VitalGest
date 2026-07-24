using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Application.DTOs.Atests;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de atestados médicos.
/// </summary>
[Authorize]
public class AtestsController : BaseApiController
{
    private readonly IAtestService _atestService;

    public AtestsController(IAtestService atestService)
    {
        _atestService = atestService;
    }

    /// <summary>
    /// Lista atestados de um paciente.
    /// </summary>
    [HttpGet("patient/{patientId:int}")]
    [ProducesResponseType(typeof(IEnumerable<AtestResponse>), 200)]
    public async Task<IActionResult> GetByPatient(int patientId)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _atestService.GetByPatientAsync(patientId, clinicId);
        return OkResponse(result);
    }

    /// <summary>
    /// Obtém detalhes de um atestado.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AtestResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _atestService.GetByIdAsync(id, clinicId);
        return OkResponse(result);
    }

    /// <summary>
    /// Emite um novo atestado para o paciente.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AtestResponse), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateAtestRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var userId = GetUserId();
        var result = await _atestService.CreateAsync(clinicId, userId, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
        {
            Success = true,
            Message = "Atestado emitido com sucesso.",
            Data = result
        });
    }

    /// <summary>
    /// Exclui um atestado.
    /// Apenas o médico que emitiu pode excluir.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Delete(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var userId = GetUserId();
        await _atestService.DeleteAsync(id, clinicId, userId);
        return OkResponse(new { }, "Atestado excluído com sucesso.");
    }
}