using VitalGest.Application.DTOs.Reports;

namespace VitalGest.Application.Interfaces;

public interface IReportService
{
    Task<FinancialReportResponse> GetFinancialReportAsync(int clinicId, DateRangeRequest request, CancellationToken ct = default);
    Task<AppointmentReportResponse> GetAppointmentReportAsync(int clinicId, DateRangeRequest request, CancellationToken ct = default);
    Task<IEnumerable<ProductionReportResponse>> GetProductionReportAsync(int clinicId, DateRangeRequest request, CancellationToken ct = default);
    Task<IEnumerable<RevenueReportResponse>> GetRevenueReportAsync(int clinicId, DateRangeRequest request, CancellationToken ct = default);
}