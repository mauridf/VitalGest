using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Application.DTOs.Prescriptions;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de prescrições médicas.
/// </summary>
[Authorize]
public class PrescriptionsController : BaseApiController
{
    private readonly IPrescriptionService _prescriptionService;

    public PrescriptionsController(IPrescriptionService prescriptionService)
    {
        _prescriptionService = prescriptionService;
    }

    /// <summary>
    /// Lista prescrições de um paciente.
    /// </summary>
    [HttpGet("patient/{patientId:int}")]
    [ProducesResponseType(typeof(IEnumerable<PrescriptionResponse>), 200)]
    public async Task<IActionResult> GetByPatient(int patientId)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _prescriptionService.GetByPatientAsync(patientId, clinicId);
        return OkResponse(result);
    }

    /// <summary>
    /// Obtém detalhes de uma prescrição com seus itens.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PrescriptionResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _prescriptionService.GetByIdAsync(id, clinicId);
        return OkResponse(result);
    }

    /// <summary>
    /// Cria uma nova prescrição com seus itens.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PrescriptionResponse), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreatePrescriptionRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var userId = GetUserId();
        var result = await _prescriptionService.CreateAsync(clinicId, userId, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
        {
            Success = true,
            Message = "Prescrição criada com sucesso.",
            Data = result
        });
    }

    /// <summary>
    /// Atualiza uma prescrição.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PrescriptionResponse), 200)]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePrescriptionRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _prescriptionService.UpdateAsync(id, clinicId, request);
        return OkResponse(result, "Prescrição atualizada com sucesso.");
    }

    /// <summary>
    /// Exclui uma prescrição.
    /// Apenas o médico que a criou pode excluir.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Delete(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var userId = GetUserId();
        await _prescriptionService.DeleteAsync(id, clinicId, userId);
        return OkResponse(new { }, "Prescrição excluída com sucesso.");
    }
}