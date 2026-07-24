using VitalGest.Application.DTOs.MedicalRecords;

namespace VitalGest.Application.Interfaces;

public interface IMedicalRecordService
{
    Task<MedicalRecordResponse> GetByPatientAsync(int patientId, int clinicId, CancellationToken ct = default);
    Task<MedicalRecordEntryResponse> AddEntryAsync(int clinicId, int doctorUserId, CreateMedicalRecordEntryRequest request, CancellationToken ct = default);
    Task<MedicalRecordEntryResponse> GetEntryAsync(int entryId, CancellationToken ct = default);
    Task<ClinicalSummaryResponse> GetSummaryAsync(int patientId, int clinicId, CancellationToken ct = default);
    Task<MedicalRecordEntryResponse> UpdateEntryAsync(int entryId, int clinicId, UpdateMedicalRecordEntryRequest request, CancellationToken ct = default);
}