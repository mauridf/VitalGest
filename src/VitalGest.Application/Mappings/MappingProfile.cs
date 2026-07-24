using AutoMapper;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;
using VitalGest.Application.DTOs.Auth;
using VitalGest.Application.DTOs.Clinics;
using VitalGest.Application.DTOs.Patients;
using VitalGest.Application.DTOs.Appointments;
using VitalGest.Application.DTOs.Schedule;
using VitalGest.Application.DTOs.MedicalRecords;
using VitalGest.Application.DTOs.Exams;
using VitalGest.Application.DTOs.Prescriptions;
using VitalGest.Application.DTOs.Atests;
using VitalGest.Application.DTOs.Financial;
using VitalGest.Application.DTOs.Insurance;
using VitalGest.Application.DTOs.Documents;
using VitalGest.Application.DTOs.Notifications;
using VitalGest.Application.DTOs.Common;
using VitalGest.Application.DTOs.Audit;

namespace VitalGest.Application.Mappings;

/// <summary>
/// Perfil de mapeamento do AutoMapper.
/// Define todos os mapeamentos entre Entities e DTOs.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ===== Auth =====
        CreateMap<User, UserProfileResponse>();
        CreateMap<ClinicUser, ClinicInfoResponse>()
            .ForMember(d => d.ClinicId, o => o.MapFrom(s => s.ClinicId))
            .ForMember(d => d.ClinicName, o => o.MapFrom(s => s.Clinic.Name))
            .ForMember(d => d.Position, o => o.MapFrom(s => s.Position.Name))
            .ForMember(d => d.Department, o => o.MapFrom(s => s.Department != null ? s.Department.Name : null));

        // ===== Clinics =====
        CreateMap<CreateClinicRequest, Clinic>();
        CreateMap<Clinic, ClinicResponse>();
        CreateMap<Department, DepartmentResponse>();
        CreateMap<CreateDepartmentRequest, Department>();

        // ===== Address =====
        CreateMap<Address, AddressResponse>();
        CreateMap<CreateAddressRequest, Address>();

        // ===== Patients =====
        CreateMap<CreatePatientRequest, Patient>();
        CreateMap<Patient, PatientResponse>()
            .ForMember(d => d.Gender, o => o.MapFrom(s => s.Gender.HasValue ? s.Gender.ToString() : null))
            .ForMember(d => d.BloodType, o => o.MapFrom(s => s.BloodType.HasValue ? s.BloodType.ToString() : null));
        CreateMap<Patient, PatientListResponse>();
        CreateMap<Patient, PatientHistoryResponse>();

        // ===== Appointments =====
        CreateMap<CreateAppointmentRequest, Appointment>();
        CreateMap<Appointment, AppointmentResponse>()
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient.Name))
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.Name))
            .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.TypeName, o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.SpecialtyName, o => o.MapFrom(s => s.Specialty != null ? s.Specialty.Name : null));
        CreateMap<Appointment, AppointmentSimpleResponse>()
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient.Name))
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.Name))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        // ===== Schedule =====
        CreateMap<CreateScheduleRequest, Schedule>();
        CreateMap<Schedule, ScheduleResponse>()
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.Name));
        CreateMap<TimeSlot, TimeSlotResponse>()
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.Name));

        // ===== Medical Records =====
        CreateMap<MedicalRecord, MedicalRecordResponse>()
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient.Name));
        CreateMap<MedicalRecordEntry, MedicalRecordEntryResponse>()
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.Name))
            .ForMember(d => d.EntryType, o => o.MapFrom(s => s.EntryType.ToString()));

        // ===== Exams =====
        CreateMap<CreateExamRequest, Exam>();
        CreateMap<Exam, ExamResponse>()
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient.Name))
            .ForMember(d => d.ExamTypeName, o => o.MapFrom(s => s.ExamType.Name))
            .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()));
        CreateMap<ExamResult, ExamResultResponse>()
            .ForMember(d => d.PerformedByName, o => o.MapFrom(s => s.PerformedBy != null ? s.PerformedBy.Name : null))
            .ForMember(d => d.ReviewedByName, o => o.MapFrom(s => s.ReviewedBy != null ? s.ReviewedBy.Name : null));
        CreateMap<Exam, ExamSimpleResponse>()
            .ForMember(d => d.ExamTypeName, o => o.MapFrom(s => s.ExamType.Name))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
        CreateMap<ExamType, ExamTypeResponse>();

        // ===== Prescriptions =====
        CreateMap<CreatePrescriptionRequest, Prescription>();
        CreateMap<Prescription, PrescriptionResponse>()
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient.Name))
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.Name));
        CreateMap<PrescriptionItem, PrescriptionItemResponse>();
        CreateMap<Prescription, PrescriptionSimpleResponse>()
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.Name))
            .ForMember(d => d.ItemCount, o => o.MapFrom(s => s.Items.Count));

        // ===== Atests =====
        CreateMap<CreateAtestRequest, Atest>();
        CreateMap<Atest, AtestResponse>()
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient.Name))
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.Name));

        // ===== Financial =====
        CreateMap<CreatePaymentRequest, Payment>()
            .ForMember(d => d.TotalAmount, o => o.MapFrom(s => s.Amount - s.Discount));
        CreateMap<Payment, PaymentResponse>()
            .ForMember(d => d.PaymentMethod, o => o.MapFrom(s => s.PaymentMethod.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient != null ? s.Patient.Name : null));
        CreateMap<CreateInvoiceRequest, Invoice>();
        CreateMap<Invoice, InvoiceResponse>()
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient != null ? s.Patient.Name : null))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        // ===== Insurance =====
        CreateMap<CreateInsurancePlanRequest, InsurancePlan>();
        CreateMap<InsurancePlan, InsurancePlanResponse>()
            .ForMember(d => d.ContractType, o => o.MapFrom(s => s.ContractType.ToString()));
        CreateMap<InsurancePlan, InsurancePlanSimpleResponse>();
        CreateMap<CreateInsuranceCoverageRequest, InsuranceCoverage>();
        CreateMap<InsuranceCoverage, InsuranceCoverageResponse>()
            .ForMember(d => d.ExamTypeName, o => o.MapFrom(s => s.ExamType != null ? s.ExamType.Name : null))
            .ForMember(d => d.SpecialtyName, o => o.MapFrom(s => s.Specialty != null ? s.Specialty.Name : null));

        // ===== Documents =====
        CreateMap<Document, DocumentResponse>()
            .ForMember(d => d.DocumentType, o => o.MapFrom(s => s.DocumentType.ToString()));

        // ===== Notifications =====
        CreateMap<Notification, NotificationResponse>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()));

        // ===== Audit =====
        CreateMap<AuditLog, AuditLogResponse>()
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.User != null ? s.User.Name : null));

        // ===== Specialties & Positions =====
        CreateMap<Specialty, Specialty>();
        CreateMap<Position, Position>();
    }
}