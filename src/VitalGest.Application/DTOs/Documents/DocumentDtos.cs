using VitalGest.Core.Enums;

namespace VitalGest.Application.DTOs.Documents;

public record DocumentResponse(int Id, string FileName, string FileUrl, long? FileSize, string? ContentType, string DocumentType, DateTime CreatedAt, int? PatientId, int? AppointmentId);