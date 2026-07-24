using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Application.DTOs.Reports;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de relatórios gerenciais.
/// Acesso restrito a Admin.
/// </summary>
[Authorize(Policy = "AdminOnly")]
public class ReportsController : BaseApiController
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Relatório financeiro por período.
    /// </summary>
    [HttpGet("financial")]
    [ProducesResponseType(typeof(FinancialReportResponse), 200)]
    public async Task<IActionResult> GetFinancialReport([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _reportService.GetFinancialReportAsync(clinicId, new DateRangeRequest(startDate, endDate));
        return OkResponse(result);
    }

    /// <summary>
    /// Relatório de agendamentos por período.
    /// </summary>
    [HttpGet("appointments")]
    [ProducesResponseType(typeof(AppointmentReportResponse), 200)]
    public async Task<IActionResult> GetAppointmentReport([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _reportService.GetAppointmentReportAsync(clinicId, new DateRangeRequest(startDate, endDate));
        return OkResponse(result);
    }

    /// <summary>
    /// Relatório de produção por médico.
    /// </summary>
    [HttpGet("production")]
    [ProducesResponseType(typeof(IEnumerable<ProductionReportResponse>), 200)]
    public async Task<IActionResult> GetProductionReport([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _reportService.GetProductionReportAsync(clinicId, new DateRangeRequest(startDate, endDate));
        return OkResponse(result);
    }

    /// <summary>
    /// Relatório de faturamento por período.
    /// </summary>
    [HttpGet("revenue")]
    [ProducesResponseType(typeof(IEnumerable<RevenueReportResponse>), 200)]
    public async Task<IActionResult> GetRevenueReport([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _reportService.GetRevenueReportAsync(clinicId, new DateRangeRequest(startDate, endDate));
        return OkResponse(result);
    }

    /// <summary>
    /// Relatório de exames realizados.
    /// </summary>
    [HttpGet("exams")]
    [ProducesResponseType(typeof(ExamReportResponse), 200)]
    public async Task<IActionResult> GetExamReport([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _reportService.GetExamReportAsync(clinicId, new DateRangeRequest(startDate, endDate));
        return OkResponse(result);
    }

    /// <summary>
    /// Relatório de pacientes.
    /// </summary>
    [HttpGet("patients")]
    [ProducesResponseType(typeof(PatientReportResponse), 200)]
    public async Task<IActionResult> GetPatientReport([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _reportService.GetPatientReportAsync(clinicId, new DateRangeRequest(startDate, endDate));
        return OkResponse(result);
    }
}