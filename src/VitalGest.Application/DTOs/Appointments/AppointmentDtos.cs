using VitalGest.Core.Enums;

namespace VitalGest.Application.DTOs.Appointments;

// ===== Requests =====

public record CreateAppointmentRequest(
    int PatientId,
    int DoctorUserId,
    DateOnly AppointmentDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    AppointmentType Type = AppointmentType.Consultation,
    int? DepartmentId = null,
    int? SpecialtyId = null,
    string? Notes = null
);

public record UpdateAppointmentRequest(
    DateOnly AppointmentDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    AppointmentType Type = AppointmentType.Consultation,
    int? DepartmentId = null,
    int? SpecialtyId = null,
    string? Notes = null,
    string? InternalNotes = null
);

public record UpdateAppointmentStatusRequest(
    AppointmentStatus Status,
    string? CancelReason = null
);

// ===== Responses =====

public record AppointmentResponse(
    int Id,
    int PatientId,
    string PatientName,
    int DoctorUserId,
    string DoctorName,
    DateOnly AppointmentDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    AppointmentStatus Status,
    string StatusName,
    AppointmentType Type,
    string TypeName,
    string? SpecialtyName,
    string? Notes,
    bool IsConfirmed,
    DateTime? ConfirmedAt,
    DateTime CreatedAt
);

public record AppointmentSimpleResponse(
    int Id,
    string PatientName,
    string DoctorName,
    DateOnly AppointmentDate,
    TimeOnly StartTime,
    string Status
);