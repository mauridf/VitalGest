namespace VitalGest.Application.DTOs.Prescriptions;

public record PrescriptionItemRequest(string MedicationName, string Dosage, string Frequency, string? Duration = null, string? Notes = null, int OrderNumber = 1);
public record CreatePrescriptionRequest(int PatientId, List<PrescriptionItemRequest> Items, string? Notes = null, int? AppointmentId = null);
public record PrescriptionItemResponse(int Id, string MedicationName, string Dosage, string Frequency, string? Duration, int OrderNumber);
public record PrescriptionResponse(int Id, int PatientId, string PatientName, int DoctorUserId, string DoctorName, DateTime IssueDate, DateOnly? ValidUntil, IEnumerable<PrescriptionItemResponse> Items, string? Notes);
public record PrescriptionSimpleResponse(int Id, string DoctorName, DateTime IssueDate, int ItemCount);