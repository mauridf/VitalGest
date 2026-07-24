using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Application.DTOs.MedicalRecords;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de prontuário eletrônico do paciente (PEP).
/// </summary>
[Authorize]
public class MedicalRecordsController : BaseApiController
{
    private readonly IMedicalRecordService _medicalRecordService;

    public MedicalRecordsController(IMedicalRecordService medicalRecordService)
    {
        _medicalRecordService = medicalRecordService;
    }

    /// <summary>
    /// Obtém o prontuário completo do paciente.
    /// Se não existir, cria um novo automaticamente.
    /// </summary>
    [HttpGet("patient/{patientId:int}")]
    [ProducesResponseType(typeof(MedicalRecordResponse), 200)]
    public async Task<IActionResult> GetByPatient(int patientId)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _medicalRecordService.GetByPatientAsync(patientId, clinicId);
        return OkResponse(result);
    }

    /// <summary>
    /// Adiciona uma nova entrada ao prontuário do paciente.
    /// </summary>
    [HttpPost("entries")]
    [ProducesResponseType(typeof(MedicalRecordEntryResponse), 201)]
    public async Task<IActionResult> AddEntry([FromBody] CreateMedicalRecordEntryRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var userId = GetUserId();
        var result = await _medicalRecordService.AddEntryAsync(clinicId, userId, request);
        return CreatedAtAction(nameof(GetEntry), new { id = result.Id }, new
        {
            Success = true,
            Message = "Entrada registrada no prontuário.",
            Data = result
        });
    }

    /// <summary>
    /// Obtém detalhes de uma entrada específica do prontuário.
    /// </summary>
    [HttpGet("entries/{id:int}")]
    [ProducesResponseType(typeof(MedicalRecordEntryResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetEntry(int id)
    {
        var result = await _medicalRecordService.GetEntryAsync(id);
        return OkResponse(result);
    }

    /// <summary>
    /// Obtém a timeline cronológica do paciente.
    /// </summary>
    [HttpGet("patient/{patientId:int}/timeline")]
    [ProducesResponseType(typeof(IEnumerable<MedicalRecordEntryResponse>), 200)]
    public async Task<IActionResult> GetTimeline(int patientId)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var record = await _medicalRecordService.GetByPatientAsync(patientId, clinicId);
        return OkResponse(record.Entries);
    }

    /// <summary>
    /// Obtém o resumo clínico do paciente.
    /// Inclui tipo sanguíneo, alergias e último atendimento.
    /// </summary>
    [HttpGet("patient/{patientId:int}/summary")]
    [ProducesResponseType(typeof(ClinicalSummaryResponse), 200)]
    public async Task<IActionResult> GetSummary(int patientId)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _medicalRecordService.GetSummaryAsync(patientId, clinicId);
        return OkResponse(result);
    }
}