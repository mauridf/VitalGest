using VitalGest.Application.DTOs.Dashboard;

namespace VitalGest.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardOverviewResponse> GetOverviewAsync(int clinicId, CancellationToken ct = default);
    Task<DashboardTodayResponse> GetTodayAsync(int clinicId, CancellationToken ct = default);
    Task<DashboardIndicatorsResponse> GetIndicatorsAsync(int clinicId, CancellationToken ct = default);
}