namespace VitalGest.Application.DTOs.Reports;

public record DateRangeRequest(DateOnly StartDate, DateOnly EndDate);
public record FinancialReportResponse(decimal TotalRevenue, decimal TotalByInsurance, decimal TotalByPrivate, int TotalTransactions);
public record AppointmentReportResponse(int Total, int Completed, int Cancelled, int NoShow, double AttendanceRate);
public record ProductionReportResponse(int DoctorUserId, string DoctorName, int TotalAppointments, decimal TotalRevenue);
public record RevenueReportResponse(string Period, decimal Amount);
public record ExamReportResponse(int TotalExams, int CompletedExams, int PendingExams);
public record PatientReportResponse(int TotalPatients, int NewPatients, int ActivePatients);