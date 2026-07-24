using VitalGest.Application.DTOs.Appointments;
using VitalGest.Application.DTOs.Dashboard;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Enums;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;

    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(2);

    public DashboardService(IUnitOfWork uow, ICacheService cache)
    {
        _uow = uow;
        _cache = cache;
    }

    public async Task<DashboardOverviewResponse> GetOverviewAsync(int clinicId, CancellationToken ct = default)
    {
        var cacheKey = $"dashboard:overview:{clinicId}";
        var cached = await _cache.GetAsync<DashboardOverviewResponse>(cacheKey, ct);
        if (cached != null) return cached;

        var todayAppointments = await _uow.Appointments.CountTodayAsync(clinicId, ct);
        var totalPatients = await _uow.Patients.CountAsync(p => p.ClinicId == clinicId && p.IsActive, ct);
        var newPatients = await _uow.Patients.CountAsync(p => p.ClinicId == clinicId && p.CreatedAt >= DateTime.UtcNow.AddDays(-30), ct);

        var result = new DashboardOverviewResponse(todayAppointments, 0, newPatients, 0, 0);
        await _cache.SetAsync(cacheKey, result, CacheTtl, ct);
        return result;
    }

    public async Task<DashboardTodayResponse> GetTodayAsync(int clinicId, CancellationToken ct = default)
    {
        var cacheKey = $"dashboard:today:{clinicId}";
        var cached = await _cache.GetAsync<DashboardTodayResponse>(cacheKey, ct);
        if (cached != null) return cached;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var appointments = await _uow.Appointments.GetByDateAsync(today, clinicId, ct);

        var items = appointments.Select(a => new AppointmentSimpleResponse(
            a.Id, a.Patient.Name, a.Doctor.Name, a.AppointmentDate, a.StartTime, a.Status.ToString())).ToList();

        var result = new DashboardTodayResponse(items, items.Count, items.Count(a => a.Status == "Scheduled"));
        await _cache.SetAsync(cacheKey, result, CacheTtl, ct);
        return result;
    }

    public async Task<DashboardIndicatorsResponse> GetIndicatorsAsync(int clinicId, CancellationToken ct = default)
    {
        var cacheKey = $"dashboard:indicators:{clinicId}";
        var cached = await _cache.GetAsync<DashboardIndicatorsResponse>(cacheKey, ct);
        if (cached != null) return cached;

        var totalPatients = await _uow.Patients.CountAsync(p => p.ClinicId == clinicId && p.IsActive, ct);
        var totalAppointments = await _uow.Appointments.CountAsync(a => a.ClinicId == clinicId, ct);

        var result = new DashboardIndicatorsResponse(totalPatients, totalAppointments, 0, 0);
        await _cache.SetAsync(cacheKey, result, CacheTtl, ct);
        return result;
    }

    public async Task<decimal> GetMonthlyRevenueAsync(int clinicId, CancellationToken ct = default)
    {
        var cacheKey = $"dashboard:revenue:monthly:{clinicId}";
        var cached = await _cache.GetAsync<string>(cacheKey, ct);
        if (cached != null && decimal.TryParse(cached, out var parsed)) return parsed;

        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var payments = await _uow.Payments.FindAsync(
            p => p.ClinicId == clinicId && p.Status == PaymentStatus.Paid && p.PaymentDate >= startOfMonth, ct);

        var total = payments.Sum(p => p.TotalAmount);
        await _cache.SetAsync(cacheKey, total.ToString(), CacheTtl, ct);
        return total;
    }

    public async Task<int> GetNewPatientsAsync(int clinicId, CancellationToken ct = default)
    {
        var cacheKey = $"dashboard:patients:new:{clinicId}";
        var cached = await _cache.GetAsync<string>(cacheKey, ct);
        if (cached != null && int.TryParse(cached, out var parsed)) return parsed;

        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var count = await _uow.Patients.CountAsync(
            p => p.ClinicId == clinicId && p.CreatedAt >= startOfMonth, ct);

        await _cache.SetAsync(cacheKey, count.ToString(), CacheTtl, ct);
        return count;
    }
}