using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Application.DTOs.Insurance;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de gestão de convênios e planos de saúde.
/// </summary>
[Authorize]
public class InsurancePlansController : BaseApiController
{
    private readonly IInsuranceService _insuranceService;

    public InsurancePlansController(IInsuranceService insuranceService)
    {
        _insuranceService = insuranceService;
    }

    /// <summary>
    /// Lista todos os planos de saúde cadastrados.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InsurancePlanResponse>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _insuranceService.GetAllAsync();
        return OkResponse(result);
    }

    /// <summary>
    /// Obtém detalhes de um plano de saúde.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(InsurancePlanResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _insuranceService.GetByIdAsync(id);
        return OkResponse(result);
    }

    /// <summary>
    /// Cadastra um novo plano de saúde.
    /// Apenas Admin pode cadastrar.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(InsurancePlanResponse), 201)]
    public async Task<IActionResult> Create([FromBody] CreateInsurancePlanRequest request)
    {
        var result = await _insuranceService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
        {
            Success = true,
            Message = "Plano de saúde cadastrado com sucesso.",
            Data = result
        });
    }

    /// <summary>
    /// Atualiza dados de um plano de saúde.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(InsurancePlanResponse), 200)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInsurancePlanRequest request)
    {
        var result = await _insuranceService.UpdateAsync(id, request);
        return OkResponse(result, "Plano de saúde atualizado com sucesso.");
    }

    /// <summary>
    /// Lista coberturas de um plano de saúde.
    /// </summary>
    [HttpGet("{id:int}/coverages")]
    [ProducesResponseType(typeof(IEnumerable<InsuranceCoverageResponse>), 200)]
    public async Task<IActionResult> GetCoverages(int id)
    {
        var result = await _insuranceService.GetCoveragesAsync(id);
        return OkResponse(result);
    }

    /// <summary>
    /// Adiciona uma cobertura ao plano de saúde.
    /// </summary>
    [HttpPost("{id:int}/coverages")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(InsuranceCoverageResponse), 201)]
    public async Task<IActionResult> AddCoverage(int id, [FromBody] CreateInsuranceCoverageRequest request)
    {
        var result = await _insuranceService.AddCoverageAsync(id, request);
        return CreatedAtAction(nameof(GetCoverages), new { id }, new
        {
            Success = true,
            Message = "Cobertura adicionada com sucesso.",
            Data = result
        });
    }
}