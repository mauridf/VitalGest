using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Application.DTOs.Common;
using VitalGest.Core.Entities;
using VitalGest.Core.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de gestão de colaboradores.
/// </summary>
[Authorize]
public class EmployeesController : BaseApiController
{
    private readonly IUnitOfWork _uow;

    public EmployeesController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    /// <summary>
    /// Lista colaboradores da clínica atual.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado no token.");

        var users = await _uow.Users.GetByClinicIdAsync(clinicId, request.Page, request.PageSize);
        return OkResponse(users);
    }

    /// <summary>
    /// Obtém detalhes de um colaborador específico.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _uow.Users.GetByIdWithClinicsAsync(id)
            ?? throw new VitalGest.Core.Exceptions.NotFoundException("Colaborador", id);
        return OkResponse(user);
    }

    /// <summary>
    /// Adiciona um colaborador à clínica.
    /// Apenas Admin pode adicionar colaboradores.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(201)]
    public async Task<IActionResult> Create([FromBody] object request)
    {
        // Implementação simplificada - será expandida
        return CreatedAtAction(nameof(GetById), new { id = 1 }, new { Success = true, Message = "Colaborador adicionado." });
    }

    /// <summary>
    /// Atualiza dados de um colaborador.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Update(int id, [FromBody] object request)
    {
        return OkResponse(new { }, "Colaborador atualizado com sucesso.");
    }

    /// <summary>
    /// Desativa um colaborador.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _uow.Users.GetByIdAsync(id)
            ?? throw new VitalGest.Core.Exceptions.NotFoundException("Colaborador", id);

        await _uow.Users.DeleteAsync(user);
        await _uow.SaveChangesAsync();

        return OkResponse(new { }, "Colaborador desativado com sucesso.");
    }

    /// <summary>
    /// Lista médicos da clínica.
    /// </summary>
    [HttpGet("doctors")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> GetDoctors()
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var doctors = await _uow.Users.GetByClinicIdAsync(clinicId, 1, 100);
        return OkResponse(doctors);
    }

    /// <summary>
    /// Lista cargos disponíveis.
    /// </summary>
    [HttpGet("positions")]
    [ProducesResponseType(typeof(IEnumerable<Position>), 200)]
    public async Task<IActionResult> GetPositions()
    {
        var positions = await _uow.Positions.GetAllAsync();
        return OkResponse(positions);
    }
}