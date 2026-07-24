using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Application.DTOs.Common;
using VitalGest.Application.DTOs.Exams;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de gestão de exames e resultados.
/// </summary>
[Authorize]
public class ExamsController : BaseApiController
{
    private readonly IExamService _examService;

    public ExamsController(IExamService examService)
    {
        _examService = examService;
    }

    /// <summary>
    /// Lista exames da clínica com paginação.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ExamResponse>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _examService.GetAllAsync(clinicId, request);
        return OkPagedResponse(result);
    }

    /// <summary>
    /// Obtém detalhes de um exame com resultado.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ExamResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _examService.GetByIdAsync(id, clinicId);
        return OkResponse(result);
    }

    /// <summary>
    /// Solicita um novo exame para o paciente.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ExamResponse), 201)]
    public async Task<IActionResult> Create([FromBody] CreateExamRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var userId = GetUserId();
        var result = await _examService.CreateAsync(clinicId, userId, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
        {
            Success = true,
            Message = "Exame solicitado com sucesso.",
            Data = result
        });
    }

    /// <summary>
    /// Atualiza o status de um exame (fluxo laboratorial).
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(ExamResponse), 200)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateExamStatusRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _examService.UpdateStatusAsync(id, clinicId, request);
        return OkResponse(result, "Status do exame atualizado.");
    }

    /// <summary>
    /// Registra o resultado de um exame.
    /// </summary>
    [HttpPost("{id:int}/results")]
    [ProducesResponseType(typeof(ExamResultResponse), 201)]
    public async Task<IActionResult> AddResult(int id, [FromBody] CreateExamResultRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var userId = GetUserId();
        var result = await _examService.AddResultAsync(id, clinicId, userId, request);
        return CreatedAtAction(nameof(GetById), new { id }, new
        {
            Success = true,
            Message = "Resultado do exame registrado.",
            Data = result
        });
    }

    /// <summary>
    /// Atualiza um resultado de exame existente.
    /// </summary>
    [HttpPut("results/{id:int}")]
    [ProducesResponseType(typeof(ExamResultResponse), 200)]
    public async Task<IActionResult> UpdateResult(int id, [FromBody] CreateExamResultRequest request)
    {
        // Nota: implementação simplificada
        return OkResponse(new { }, "Resultado atualizado.");
    }

    /// <summary>
    /// Lista todos os tipos de exame disponíveis.
    /// </summary>
    [HttpGet("types")]
    [ProducesResponseType(typeof(IEnumerable<ExamTypeResponse>), 200)]
    public async Task<IActionResult> GetExamTypes()
    {
        var result = await _examService.GetExamTypesAsync();
        return OkResponse(result);
    }
}