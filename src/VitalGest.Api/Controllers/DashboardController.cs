using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Application.DTOs.Dashboard;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller do dashboard da clínica.
/// </summary>
[Authorize]
public class DashboardController : BaseApiController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Visão geral do dashboard.
    /// </summary>
    [HttpGet("overview")]
    [ProducesResponseType(typeof(DashboardOverviewResponse), 200)]
    public async Task<IActionResult> GetOverview()
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _dashboardService.GetOverviewAsync(clinicId);
        return OkResponse(result);
    }

    /// <summary>
    /// Agenda do dia atual.
    /// </summary>
    [HttpGet("today")]
    [ProducesResponseType(typeof(DashboardTodayResponse), 200)]
    public async Task<IActionResult> GetToday()
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _dashboardService.GetTodayAsync(clinicId);
        return OkResponse(result);
    }

    /// <summary>
    /// Indicadores da clínica.
    /// </summary>
    [HttpGet("indicators")]
    [ProducesResponseType(typeof(DashboardIndicatorsResponse), 200)]
    public async Task<IActionResult> GetIndicators()
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _dashboardService.GetIndicatorsAsync(clinicId);
        return OkResponse(result);
    }

    /// <summary>
    /// Faturamento mensal.
    /// </summary>
    [HttpGet("revenue/monthly")]
    [ProducesResponseType(typeof(decimal), 200)]
    public async Task<IActionResult> GetMonthlyRevenue()
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var revenue = await _dashboardService.GetMonthlyRevenueAsync(clinicId);
        return OkResponse(new { MonthlyRevenue = revenue });
    }

    /// <summary>
    /// Novos pacientes no mês.
    /// </summary>
    [HttpGet("patients/new")]
    [ProducesResponseType(typeof(int), 200)]
    public async Task<IActionResult> GetNewPatients()
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var count = await _dashboardService.GetNewPatientsAsync(clinicId);
        return OkResponse(new { NewPatients = count });
    }
}