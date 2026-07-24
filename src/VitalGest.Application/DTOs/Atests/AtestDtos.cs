namespace VitalGest.Application.DTOs.Atests;

public record CreateAtestRequest(int PatientId, DateOnly StartDate, DateOnly EndDate, string Description, string? CID = null, int? AppointmentId = null);
public record AtestResponse(int Id, int PatientId, string PatientName, int DoctorUserId, string DoctorName, DateTime IssueDate, DateOnly StartDate, DateOnly EndDate, string? CID, string Description, int RestDays);