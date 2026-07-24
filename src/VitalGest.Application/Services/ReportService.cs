using VitalGest.Application.DTOs.Reports;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Enums;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;

    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    public ReportService(IUnitOfWork uow, ICacheService cache)
    {
        _uow = uow;
        _cache = cache;
    }

    public async Task<FinancialReportResponse> GetFinancialReportAsync(int clinicId, DateRangeRequest request, CancellationToken ct = default)
    {
        var cacheKey = $"report:financial:{clinicId}:{request.StartDate}:{request.EndDate}";
        var cached = await _cache.GetAsync<FinancialReportResponse>(cacheKey, ct);
        if (cached != null) return cached;

        var payments = await _uow.Payments.FindAsync(p => p.ClinicId == clinicId && p.Status == PaymentStatus.Paid, ct);
        var filtered = payments.Where(p => DateOnly.FromDateTime(p.PaymentDate) >= request.StartDate && DateOnly.FromDateTime(p.PaymentDate) <= request.EndDate);

        var result = new FinancialReportResponse(
            filtered.Sum(p => p.TotalAmount),
            filtered.Where(p => p.PaymentMethod == PaymentMethod.Insurance).Sum(p => p.TotalAmount),
            filtered.Where(p => p.PaymentMethod != PaymentMethod.Insurance).Sum(p => p.TotalAmount),
            filtered.Count()
        );

        await _cache.SetAsync(cacheKey, result, CacheTtl, ct);
        return result;
    }

    public async Task<AppointmentReportResponse> GetAppointmentReportAsync(int clinicId, DateRangeRequest request, CancellationToken ct = default)
    {
        var cacheKey = $"report:appointments:{clinicId}:{request.StartDate}:{request.EndDate}";
        var cached = await _cache.GetAsync<AppointmentReportResponse>(cacheKey, ct);
        if (cached != null) return cached;

        var appointments = await _uow.Appointments.FindAsync(a => a.ClinicId == clinicId, ct);
        var filtered = appointments.Where(a => a.AppointmentDate >= request.StartDate && a.AppointmentDate <= request.EndDate).ToList();

        var total = filtered.Count;
        var completed = filtered.Count(a => a.Status == AppointmentStatus.Completed);

        var result = new AppointmentReportResponse(
            total,
            completed,
            filtered.Count(a => a.Status == AppointmentStatus.Cancelled),
            filtered.Count(a => a.Status == AppointmentStatus.NoShow),
            total > 0 ? Math.Round((double)completed / total * 100, 2) : 0
        );

        await _cache.SetAsync(cacheKey, result, CacheTtl, ct);
        return result;
    }

    public async Task<IEnumerable<ProductionReportResponse>> GetProductionReportAsync(int clinicId, DateRangeRequest request, CancellationToken ct = default)
    {
        var cacheKey = $"report:production:{clinicId}:{request.StartDate}:{request.EndDate}";
        var cached = await _cache.GetAsync<IEnumerable<ProductionReportResponse>>(cacheKey, ct);
        if (cached != null) return cached;

        var appointments = await _uow.Appointments.FindAsync(a => a.ClinicId == clinicId, ct);
        var filtered = appointments.Where(a => a.AppointmentDate >= request.StartDate && a.AppointmentDate <= request.EndDate);

        var result = filtered.GroupBy(a => a.DoctorUserId)
            .Select(g => new ProductionReportResponse(g.Key, g.FirstOrDefault()?.Doctor.Name ?? "N/A", g.Count(), 0))
            .ToList();

        await _cache.SetAsync(cacheKey, result.AsEnumerable(), CacheTtl, ct);
        return result;
    }

    public Task<IEnumerable<RevenueReportResponse>> GetRevenueReportAsync(int clinicId, DateRangeRequest request, CancellationToken ct = default)
    {
        return Task.FromResult(Enumerable.Empty<RevenueReportResponse>());
    }

    public async Task<ExamReportResponse> GetExamReportAsync(int clinicId, DateRangeRequest request, CancellationToken ct = default)
    {
        var cacheKey = $"report:exams:{clinicId}:{request.StartDate}:{request.EndDate}";
        var cached = await _cache.GetAsync<ExamReportResponse>(cacheKey, ct);
        if (cached != null) return cached;

        var exams = await _uow.Exams.FindAsync(e => e.ClinicId == clinicId, ct);
        var filtered = exams.Where(e => DateOnly.FromDateTime(e.RequestDate) >= request.StartDate && DateOnly.FromDateTime(e.RequestDate) <= request.EndDate).ToList();

        var result = new ExamReportResponse(
            filtered.Count,
            filtered.Count(e => e.Status == ExamStatus.Ready || e.Status == ExamStatus.Delivered),
            filtered.Count(e => e.Status == ExamStatus.Requested || e.Status == ExamStatus.Collected || e.Status == ExamStatus.InAnalysis)
        );

        await _cache.SetAsync(cacheKey, result, CacheTtl, ct);
        return result;
    }

    public async Task<PatientReportResponse> GetPatientReportAsync(int clinicId, DateRangeRequest request, CancellationToken ct = default)
    {
        var cacheKey = $"report:patients:{clinicId}:{request.StartDate}:{request.EndDate}";
        var cached = await _cache.GetAsync<PatientReportResponse>(cacheKey, ct);
        if (cached != null) return cached;

        var patients = await _uow.Patients.FindAsync(p => p.ClinicId == clinicId, ct);
        var filtered = patients.Where(p => DateOnly.FromDateTime(p.CreatedAt) >= request.StartDate && DateOnly.FromDateTime(p.CreatedAt) <= request.EndDate).ToList();

        var result = new PatientReportResponse(
            filtered.Count,
            filtered.Count(p => p.CreatedAt >= DateTime.UtcNow.AddDays(-30)),
            patients.Count(p => p.IsActive)
        );

        await _cache.SetAsync(cacheKey, result, CacheTtl, ct);
        return result;
    }
}