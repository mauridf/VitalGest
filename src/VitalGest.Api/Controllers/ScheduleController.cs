using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Application.DTOs.Schedule;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de gestão de agendas dos profissionais.
/// </summary>
[Authorize]
public class ScheduleController : BaseApiController
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    /// <summary>
    /// Obtém a agenda de um médico.
    /// </summary>
    [HttpGet("doctor/{doctorId:int}")]
    [ProducesResponseType(typeof(IEnumerable<ScheduleResponse>), 200)]
    public async Task<IActionResult> GetByDoctor(int doctorId)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _scheduleService.GetByDoctorAsync(clinicId, doctorId);
        return OkResponse(result);
    }

    /// <summary>
    /// Cria uma nova regra de agenda para um médico.
    /// Apenas Admin pode criar.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ScheduleResponse), 201)]
    public async Task<IActionResult> Create([FromBody] CreateScheduleRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _scheduleService.CreateAsync(clinicId, request);
        return CreatedAtAction(nameof(GetByDoctor), new { doctorId = request.DoctorUserId }, new
        {
            Success = true,
            Message = "Regra de agenda criada com sucesso.",
            Data = result
        });
    }

    /// <summary>
    /// Atualiza uma regra de agenda existente.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ScheduleResponse), 200)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateScheduleRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _scheduleService.UpdateAsync(id, clinicId, request);
        return OkResponse(result, "Regra atualizada com sucesso.");
    }

    /// <summary>
    /// Remove uma regra de agenda.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Delete(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        await _scheduleService.DeleteAsync(id, clinicId);
        return OkResponse(new { }, "Regra removida com sucesso.");
    }

    /// <summary>
    /// Cria uma exceção de agenda (folga, bloqueio).
    /// </summary>
    [HttpPost("exceptions")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ScheduleExceptionResponse), 201)]
    public async Task<IActionResult> CreateException([FromBody] CreateScheduleExceptionRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _scheduleService.CreateExceptionAsync(clinicId, request);
        return CreatedAtAction(nameof(GetByDoctor), new { doctorId = request.DoctorUserId }, new
        {
            Success = true,
            Message = "Exceção criada com sucesso.",
            Data = result
        });
    }

    /// <summary>
    /// Remove uma exceção de agenda.
    /// </summary>
    [HttpDelete("exceptions/{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> DeleteException(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        await _scheduleService.DeleteExceptionAsync(id, clinicId);
        return OkResponse(new { }, "Exceção removida com sucesso.");
    }

    /// <summary>
    /// Obtém horários disponíveis de um médico em uma data.
    /// Endpoint público para agendamento online.
    /// </summary>
    [HttpGet("slots")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<TimeSlotResponse>), 200)]
    public async Task<IActionResult> GetAvailableSlots([FromQuery] int doctorId, [FromQuery] DateOnly date)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _scheduleService.GetAvailableSlotsAsync(clinicId, doctorId, date);
        return OkResponse(result);
    }
}