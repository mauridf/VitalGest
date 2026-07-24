using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Application.DTOs.Appointments;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de gestão de agendamentos.
/// </summary>
[Authorize]
public class AppointmentsController : BaseApiController
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    /// <summary>
    /// Lista agendamentos da clínica.
    /// Aceita filtros por data, médico, status.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AppointmentResponse>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] DateOnly? date = null)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");

        if (date.HasValue)
        {
            var result = await _appointmentService.GetByDateAsync(clinicId, date.Value);
            return OkResponse(result);
        }

        return OkResponse(Enumerable.Empty<AppointmentResponse>());
    }

    /// <summary>
    /// Obtém detalhes de um agendamento.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AppointmentResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _appointmentService.GetByIdAsync(id, clinicId);
        return OkResponse(result);
    }

    /// <summary>
    /// Cria um novo agendamento.
    /// Verifica conflitos de horário automaticamente.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AppointmentResponse), 201)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var userId = GetUserId();

        var result = await _appointmentService.CreateAsync(clinicId, userId, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
        {
            Success = true,
            Message = "Agendamento criado com sucesso.",
            Data = result
        });
    }

    /// <summary>
    /// Atualiza dados de um agendamento.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(AppointmentResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _appointmentService.UpdateAsync(id, clinicId, request);
        return OkResponse(result, "Agendamento atualizado com sucesso.");
    }

    /// <summary>
    /// Altera o status de um agendamento.
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(AppointmentResponse), 200)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAppointmentStatusRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _appointmentService.UpdateStatusAsync(id, clinicId, request);
        return OkResponse(result, "Status atualizado com sucesso.");
    }

    /// <summary>
    /// Confirma um agendamento.
    /// </summary>
    [HttpPost("{id:int}/confirm")]
    [ProducesResponseType(typeof(AppointmentResponse), 200)]
    public async Task<IActionResult> Confirm(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _appointmentService.ConfirmAsync(id, clinicId);
        return OkResponse(result, "Agendamento confirmado.");
    }

    /// <summary>
    /// Cancela um agendamento.
    /// Requer motivo do cancelamento.
    /// </summary>
    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(typeof(AppointmentResponse), 200)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Cancel(int id, [FromBody] CancelRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _appointmentService.CancelAsync(id, clinicId, request.Reason);
        return OkResponse(result, "Agendamento cancelado.");
    }

    /// <summary>
    /// Marca agendamento como "Não Compareceu".
    /// </summary>
    [HttpPost("{id:int}/no-show")]
    [ProducesResponseType(typeof(AppointmentResponse), 200)]
    public async Task<IActionResult> MarkNoShow(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _appointmentService.MarkNoShowAsync(id, clinicId);
        return OkResponse(result, "Marcado como não compareceu.");
    }

    /// <summary>
    /// Lista agendamentos por data específica.
    /// </summary>
    [HttpGet("date/{date}")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentResponse>), 200)]
    public async Task<IActionResult> GetByDate(DateOnly date)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _appointmentService.GetByDateAsync(clinicId, date);
        return OkResponse(result);
    }

    /// <summary>
    /// Lista agendamentos de um médico específico.
    /// </summary>
    [HttpGet("doctor/{doctorId:int}")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentResponse>), 200)]
    public async Task<IActionResult> GetByDoctor(int doctorId)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _appointmentService.GetByDoctorAsync(clinicId, doctorId);
        return OkResponse(result);
    }
}

/// <summary>
/// DTO para cancelamento de agendamento.
/// </summary>
public record CancelRequest(string Reason);