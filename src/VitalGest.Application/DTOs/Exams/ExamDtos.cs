using VitalGest.Core.Enums;

namespace VitalGest.Application.DTOs.Exams;

public record CreateExamRequest(int PatientId, int ExamTypeId, string? Notes = null, string? ClinicalHistory = null, int? AppointmentId = null);
public record UpdateExamStatusRequest(ExamStatus Status);
public record CreateExamResultRequest(string? Summary = null, string? ResultJson = null, string? FileUrl = null);
public record ExamResponse(int Id, int PatientId, string PatientName, int ExamTypeId, string ExamTypeName, ExamStatus Status, string StatusName, DateTime RequestDate, string? Notes);
public record ExamResultResponse(int Id, string? Summary, string? ResultJson, string? FileUrl, DateTime ResultDate, string? PerformedByName, string? ReviewedByName);
public record ExamTypeResponse(int Id, string Name, string? Description, bool IsLaboratory, bool IsImage);
public record ExamSimpleResponse(int Id, string ExamTypeName, string Status, DateTime RequestDate);