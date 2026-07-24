using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Application.DTOs.Common;
using VitalGest.Application.DTOs.Employees;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

[Authorize]
public class EmployeesController : BaseApiController
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<EmployeeResponse>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _employeeService.GetAllAsync(clinicId, request);
        return OkPagedResponse(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmployeeResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _employeeService.GetByIdAsync(id, clinicId);
        return OkResponse(result);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(EmployeeResponse), 201)]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _employeeService.CreateAsync(clinicId, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
        {
            Success = true,
            Message = "Colaborador adicionado com sucesso.",
            Data = result
        });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(EmployeeResponse), 200)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _employeeService.UpdateAsync(id, clinicId, request);
        return OkResponse(result, "Colaborador atualizado com sucesso.");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Delete(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        await _employeeService.DeleteAsync(id, clinicId);
        return OkResponse(new { }, "Colaborador desativado com sucesso.");
    }

    [HttpGet("doctors")]
    [ProducesResponseType(typeof(IEnumerable<EmployeeResponse>), 200)]
    public async Task<IActionResult> GetDoctors()
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _employeeService.GetDoctorsAsync(clinicId);
        return OkResponse(result);
    }

    [HttpGet("positions")]
    [ProducesResponseType(typeof(IEnumerable<PositionResponse>), 200)]
    public async Task<IActionResult> GetPositions()
    {
        var result = await _employeeService.GetPositionsAsync();
        return OkResponse(result);
    }
}
