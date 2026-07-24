using VitalGest.Application.DTOs.Appointments;
using VitalGest.Application.DTOs.Common;
using VitalGest.Application.DTOs.Exams;
using VitalGest.Application.DTOs.Insurance;
using VitalGest.Application.DTOs.Prescriptions;
using VitalGest.Core.Enums;

namespace VitalGest.Application.DTOs.Patients;

// ===== Requests =====

public record CreatePatientRequest(
    string Name,
    string Phone,
    string? CPF = null,
    string? RG = null,
    DateOnly? BirthDate = null,
    Gender? Gender = null,
    string? SecondaryPhone = null,
    string? Email = null,
    CreateAddressRequest? Address = null,
    BloodType? BloodType = null,
    string? Allergies = null,
    string? MedicalNotes = null,
    string? EmergencyContact = null,
    string? EmergencyPhone = null,
    int? InsurancePlanId = null,
    string? InsuranceCardNumber = null,
    DateOnly? InsuranceExpiryDate = null
);

public record UpdatePatientRequest(
    string Name,
    string Phone,
    string? RG = null,
    DateOnly? BirthDate = null,
    Gender? Gender = null,
    string? SecondaryPhone = null,
    string? Email = null,
    BloodType? BloodType = null,
    string? Allergies = null,
    string? MedicalNotes = null,
    string? EmergencyContact = null,
    string? EmergencyPhone = null,
    int? InsurancePlanId = null,
    string? InsuranceCardNumber = null,
    DateOnly? InsuranceExpiryDate = null
);

// ===== Responses =====

public record PatientResponse(
    int Id,
    string Name,
    string? CPF,
    string? RG,
    DateOnly? BirthDate,
    string? Gender,
    string Phone,
    string? SecondaryPhone,
    string? Email,
    AddressResponse? Address,
    string? BloodType,
    string? Allergies,
    string? EmergencyContact,
    string? EmergencyPhone,
    InsurancePlanSimpleResponse? InsurancePlan,
    string? InsuranceCardNumber,
    bool IsActive,
    DateTime CreatedAt
);

public record PatientListResponse(
    int Id,
    string Name,
    string? CPF,
    string Phone,
    DateOnly? BirthDate,
    bool IsActive
);

public record PatientHistoryResponse(
    PatientResponse Patient,
    IEnumerable<AppointmentSimpleResponse> RecentAppointments,
    IEnumerable<ExamSimpleResponse> RecentExams,
    IEnumerable<PrescriptionSimpleResponse> RecentPrescriptions
);