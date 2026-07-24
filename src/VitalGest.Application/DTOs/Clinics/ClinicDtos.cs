using VitalGest.Application.DTOs.Common;

namespace VitalGest.Application.DTOs.Clinics;

// ===== Requests =====

public record CreateClinicRequest(
    string Name,
    string CorporateName,
    string CNPJ,
    string Phone,
    string Email,
    string? Description = null,
    string? SecondaryPhone = null,
    string? Website = null,
    CreateAddressRequest? Address = null,
    string? OpeningHours = null
);

public record UpdateClinicRequest(
    string Name,
    string CorporateName,
    string Phone,
    string Email,
    string? Description = null,
    string? SecondaryPhone = null,
    string? Website = null,
    string? OpeningHours = null
);

public record CreateDepartmentRequest(
    string Name,
    string? Description = null
);

public record UpdateDepartmentRequest(
    string Name,
    string? Description = null
);

// ===== Responses =====

public record ClinicResponse(
    int Id,
    string Name,
    string CorporateName,
    string CNPJ,
    string? Description,
    string? LogoUrl,
    string Phone,
    string? SecondaryPhone,
    string Email,
    string? Website,
    AddressResponse? Address,
    string? OpeningHours,
    bool IsActive,
    DateTime CreatedAt
);

public record DepartmentResponse(
    int Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAt
);

public record ClinicStatsResponse(
    int TotalPatients,
    int TotalAppointments,
    int TotalDoctors,
    int TodayAppointments,
    decimal MonthlyRevenue
);