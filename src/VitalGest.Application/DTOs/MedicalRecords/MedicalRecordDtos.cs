using VitalGest.Core.Enums;

namespace VitalGest.Application.DTOs.MedicalRecords;

public record CreateMedicalRecordEntryRequest(int PatientId, MedicalRecordEntryType EntryType, string Description, int? AppointmentId = null, bool IsConfidential = false);
public record MedicalRecordEntryResponse(int Id, int DoctorUserId, string DoctorName, string EntryType, string Description, bool IsConfidential, DateTime CreatedAt);
public record MedicalRecordResponse(int Id, int PatientId, string PatientName, IEnumerable<MedicalRecordEntryResponse> Entries, DateTime CreatedAt);
public record ClinicalSummaryResponse(string PatientName, string BloodType, string Allergies, string LastAppointment, int TotalEntries);