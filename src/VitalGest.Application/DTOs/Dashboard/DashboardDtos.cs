using VitalGest.Application.DTOs.Appointments;

namespace VitalGest.Application.DTOs.Dashboard;

public record DashboardOverviewResponse(int TodayAppointments, int WaitingPatients, int NewPatients, decimal TodayRevenue, double AttendanceRate);
public record DashboardTodayResponse(IEnumerable<AppointmentSimpleResponse> Appointments, int Total, int Waiting);
public record DashboardIndicatorsResponse(int TotalPatients, int TotalAppointments, decimal MonthlyRevenue, double SatisfactionRate);