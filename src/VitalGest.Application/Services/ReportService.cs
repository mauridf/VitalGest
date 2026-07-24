using VitalGest.Application.DTOs.Reports;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Enums;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _uow;

    public ReportService(IUnitOfWork uow) => _uow = uow;

    public async Task<FinancialReportResponse> GetFinancialReportAsync(int clinicId, DateRangeRequest request, CancellationToken ct = default)
    {
        var payments = await _uow.Payments.FindAsync(p => p.ClinicId == clinicId && p.Status == PaymentStatus.Paid, ct);
        var filtered = payments.Where(p => DateOnly.FromDateTime(p.PaymentDate) >= request.StartDate && DateOnly.FromDateTime(p.PaymentDate) <= request.EndDate);

        return new FinancialReportResponse(
            filtered.Sum(p => p.TotalAmount),
            filtered.Where(p => p.PaymentMethod == PaymentMethod.Insurance).Sum(p => p.TotalAmount),
            filtered.Where(p => p.PaymentMethod != PaymentMethod.Insurance).Sum(p => p.TotalAmount),
            filtered.Count()
        );
    }

    public async Task<AppointmentReportResponse> GetAppointmentReportAsync(int clinicId, DateRangeRequest request, CancellationToken ct = default)
    {
        var appointments = await _uow.Appointments.FindAsync(a => a.ClinicId == clinicId, ct);
        var filtered = appointments.Where(a => a.AppointmentDate >= request.StartDate && a.AppointmentDate <= request.EndDate).ToList();

        var total = filtered.Count;
        var completed = filtered.Count(a => a.Status == AppointmentStatus.Completed);

        return new AppointmentReportResponse(
            total,
            completed,
            filtered.Count(a => a.Status == AppointmentStatus.Cancelled),
            filtered.Count(a => a.Status == AppointmentStatus.NoShow),
            total > 0 ? Math.Round((double)completed / total * 100, 2) : 0
        );
    }

    public async Task<IEnumerable<ProductionReportResponse>> GetProductionReportAsync(int clinicId, DateRangeRequest request, CancellationToken ct = default)
    {
        var appointments = await _uow.Appointments.FindAsync(a => a.ClinicId == clinicId, ct);
        var filtered = appointments.Where(a => a.AppointmentDate >= request.StartDate && a.AppointmentDate <= request.EndDate);

        return filtered.GroupBy(a => a.DoctorUserId)
            .Select(g => new ProductionReportResponse(g.Key, g.FirstOrDefault()?.Doctor.Name ?? "N/A", g.Count(), 0));
    }

    public Task<IEnumerable<RevenueReportResponse>> GetRevenueReportAsync(int clinicId, DateRangeRequest request, CancellationToken ct = default)
    {
        // Simplificado - retorna vazio
        return Task.FromResult(Enumerable.Empty<RevenueReportResponse>());
    }
}