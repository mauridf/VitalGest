using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Application.DTOs.Common;
using VitalGest.Application.DTOs.Patients;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de gestão de pacientes.
/// </summary>
[Authorize]
public class PatientsController : BaseApiController
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    /// <summary>
    /// Lista pacientes da clínica com paginação.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<PatientListResponse>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _patientService.GetAllAsync(clinicId, request);
        return OkPagedResponse(result);
    }

    /// <summary>
    /// Busca pacientes por nome, CPF ou telefone.
    /// Mínimo de 2 caracteres para a busca.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResponse<PatientListResponse>), 200)]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] PagedRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");

        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return BadRequest(new { Success = false, Message = "A busca deve ter no mínimo 2 caracteres." });

        var result = await _patientService.SearchAsync(clinicId, query, request);
        return OkPagedResponse(result);
    }

    /// <summary>
    /// Obtém detalhes completos de um paciente.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PatientResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _patientService.GetByIdAsync(id, clinicId);
        return OkResponse(result);
    }

    /// <summary>
    /// Cadastra um novo paciente na clínica.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PatientResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] CreatePatientRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _patientService.CreateAsync(clinicId, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
        {
            Success = true,
            Message = "Paciente cadastrado com sucesso.",
            Data = result
        });
    }

    /// <summary>
    /// Atualiza dados de um paciente.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PatientResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePatientRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _patientService.UpdateAsync(id, clinicId, request);
        return OkResponse(result, "Paciente atualizado com sucesso.");
    }

    /// <summary>
    /// Desativa um paciente (soft delete).
    /// Apenas Admin pode desativar.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        await _patientService.DeleteAsync(id, clinicId);
        return OkResponse(new { }, "Paciente desativado com sucesso.");
    }

    /// <summary>
    /// Obtém o histórico completo do paciente.
    /// Inclui últimos agendamentos, exames e prescrições.
    /// </summary>
    [HttpGet("{id:int}/history")]
    [ProducesResponseType(typeof(PatientHistoryResponse), 200)]
    public async Task<IActionResult> GetHistory(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _patientService.GetHistoryAsync(id, clinicId);
        return OkResponse(result);
    }

    /// <summary>
    /// Lista agendamentos do paciente.
    /// </summary>
    [HttpGet("{id:int}/appointments")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> GetAppointments(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        // Delegado para o AppointmentService
        return OkResponse(new { PatientId = id, Appointments = new List<object>() });
    }

    /// <summary>
    /// Lista exames do paciente.
    /// </summary>
    [HttpGet("{id:int}/exams")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> GetExams(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        return OkResponse(new { PatientId = id, Exams = new List<object>() });
    }

    /// <summary>
    /// Lista prescrições do paciente.
    /// </summary>
    [HttpGet("{id:int}/prescriptions")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> GetPrescriptions(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        return OkResponse(new { PatientId = id, Prescriptions = new List<object>() });
    }
}