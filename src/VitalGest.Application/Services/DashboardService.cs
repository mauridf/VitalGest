using VitalGest.Application.DTOs.Appointments;
using VitalGest.Application.DTOs.Dashboard;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Enums;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _uow;

    public DashboardService(IUnitOfWork uow) => _uow = uow;

    public async Task<DashboardOverviewResponse> GetOverviewAsync(int clinicId, CancellationToken ct = default)
    {
        var todayAppointments = await _uow.Appointments.CountTodayAsync(clinicId, ct);
        var totalPatients = await _uow.Patients.CountAsync(p => p.ClinicId == clinicId && p.IsActive, ct);
        var newPatients = await _uow.Patients.CountAsync(p => p.ClinicId == clinicId && p.CreatedAt >= DateTime.UtcNow.AddDays(-30), ct);

        return new DashboardOverviewResponse(todayAppointments, 0, newPatients, 0, 0);
    }

    public async Task<DashboardTodayResponse> GetTodayAsync(int clinicId, CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var appointments = await _uow.Appointments.GetByDateAsync(today, clinicId, ct);

        var items = appointments.Select(a => new AppointmentSimpleResponse(
            a.Id, a.Patient.Name, a.Doctor.Name, a.AppointmentDate, a.StartTime, a.Status.ToString())).ToList();

        return new DashboardTodayResponse(items, items.Count, items.Count(a => a.Status == "Scheduled"));
    }

    public async Task<DashboardIndicatorsResponse> GetIndicatorsAsync(int clinicId, CancellationToken ct = default)
    {
        var totalPatients = await _uow.Patients.CountAsync(p => p.ClinicId == clinicId && p.IsActive, ct);
        var totalAppointments = await _uow.Appointments.CountAsync(a => a.ClinicId == clinicId, ct);

        return new DashboardIndicatorsResponse(totalPatients, totalAppointments, 0, 0);
    }
}