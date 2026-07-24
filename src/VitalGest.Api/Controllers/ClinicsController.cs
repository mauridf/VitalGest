using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VitalGest.Application.DTOs.Clinics;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de gestão de clínicas (tenants).
/// </summary>
[EnableRateLimiting("Public")]
public class ClinicsController : BaseApiController
{
    private readonly IClinicService _clinicService;

    public ClinicsController(IClinicService clinicService)
    {
        _clinicService = clinicService;
    }

    /// <summary>
    /// Cadastra uma nova clínica (onboarding).
    /// Endpoint público para auto-cadastro.
    /// </summary>
    /// <param name="request">Dados da clínica</param>
    /// <returns>Clínica criada</returns>
    /// <response code="201">Clínica cadastrada com sucesso</response>
    /// <response code="409">CNPJ já cadastrado</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ClinicResponse), 201)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] CreateClinicRequest request)
    {
        var result = await _clinicService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
        {
            Success = true,
            Message = "Clínica cadastrada com sucesso.",
            Data = result
        });
    }

    /// <summary>
    /// Obtém os detalhes de uma clínica específica.
    /// </summary>
    /// <param name="id">ID da clínica</param>
    /// <returns>Detalhes da clínica</returns>
    /// <response code="200">Clínica encontrada</response>
    /// <response code="404">Clínica não encontrada</response>
    [HttpGet("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ClinicResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _clinicService.GetByIdAsync(id);
        return OkResponse(result);
    }

    /// <summary>
    /// Atualiza os dados de uma clínica.
    /// Apenas Admin da clínica pode atualizar.
    /// </summary>
    /// <param name="id">ID da clínica</param>
    /// <param name="request">Novos dados</param>
    /// <returns>Clínica atualizada</returns>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ClinicResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClinicRequest request)
    {
        var result = await _clinicService.UpdateAsync(id, request);
        return OkResponse(result, "Clínica atualizada com sucesso.");
    }

    /// <summary>
    /// Obtém estatísticas da clínica.
    /// </summary>
    /// <param name="id">ID da clínica</param>
    /// <returns>Estatísticas</returns>
    [HttpGet("{id:int}/stats")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ClinicStatsResponse), 200)]
    public async Task<IActionResult> GetStats(int id)
    {
        var result = await _clinicService.GetStatsAsync(id);
        return OkResponse(result);
    }

    /// <summary>
    /// Lista departamentos da clínica.
    /// </summary>
    /// <param name="id">ID da clínica</param>
    /// <returns>Lista de departamentos</returns>
    [HttpGet("{id:int}/departments")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<DepartmentResponse>), 200)]
    public async Task<IActionResult> GetDepartments(int id)
    {
        var result = await _clinicService.GetDepartmentsAsync(id);
        return OkResponse(result);
    }

    /// <summary>
    /// Cria um novo departamento na clínica.
    /// </summary>
    /// <param name="id">ID da clínica</param>
    /// <param name="request">Dados do departamento</param>
    /// <returns>Departamento criado</returns>
    [HttpPost("{id:int}/departments")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(DepartmentResponse), 201)]
    public async Task<IActionResult> CreateDepartment(int id, [FromBody] CreateDepartmentRequest request)
    {
        var result = await _clinicService.CreateDepartmentAsync(id, request);
        return CreatedAtAction(nameof(GetDepartments), new { id }, new
        {
            Success = true,
            Message = "Departamento criado com sucesso.",
            Data = result
        });
    }
}