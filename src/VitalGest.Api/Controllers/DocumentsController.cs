using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de upload e gestão de documentos.
/// </summary>
[Authorize]
public class DocumentsController : BaseApiController
{
    private readonly IDocumentService _documentService;

    public DocumentsController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    /// <summary>
    /// Lista documentos da clínica.
    /// Aceita filtro por paciente, agendamento ou exame.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int? patientId = null, [FromQuery] int? appointmentId = null, [FromQuery] int? examId = null)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");

        if (patientId.HasValue)
        {
            var docs = await _documentService.GetByPatientAsync(patientId.Value, clinicId);
            return OkResponse(docs);
        }

        return OkResponse(Enumerable.Empty<object>());
    }

    /// <summary>
    /// Faz upload de um novo documento.
    /// Tamanho máximo: 10MB.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromQuery] int? patientId = null,
        [FromQuery] int? appointmentId = null,
        [FromQuery] int? examId = null,
        [FromQuery] int documentType = 5)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var userId = GetUserId();

        var result = await _documentService.UploadAsync(clinicId, userId, file, patientId, appointmentId, examId, documentType);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
        {
            Success = true,
            Message = "Documento enviado com sucesso.",
            Data = result
        });
    }

    /// <summary>
    /// Obtém detalhes de um documento.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _documentService.GetByIdAsync(id, clinicId);
        return OkResponse(result);
    }

    /// <summary>
    /// Exclui um documento.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Delete(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        await _documentService.DeleteAsync(id, clinicId);
        return OkResponse(new { }, "Documento excluído com sucesso.");
    }

    /// <summary>
    /// Lista documentos de um paciente específico.
    /// </summary>
    [HttpGet("patient/{patientId:int}")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> GetByPatient(int patientId)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _documentService.GetByPatientAsync(patientId, clinicId);
        return OkResponse(result);
    }

    /// <summary>
    /// Lista documentos de um agendamento específico.
    /// </summary>
    [HttpGet("appointment/{appointmentId:int}")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> GetByAppointment(int appointmentId)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _documentService.GetByAppointmentAsync(appointmentId, clinicId);
        return OkResponse(result);
    }
}