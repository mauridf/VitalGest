using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.MedicalRecords;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Entities;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

public class MedicalRecordService : IMedicalRecordService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<MedicalRecordService> _logger;

    public MedicalRecordService(IUnitOfWork uow, IMapper mapper, ILogger<MedicalRecordService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<MedicalRecordResponse> GetByPatientAsync(int patientId, int clinicId, CancellationToken ct = default)
    {
        var record = await _uow.MedicalRecords.GetByPatientIdWithEntriesAsync(patientId, clinicId, ct);
        if (record == null)
        {
            // Cria prontuário se não existir
            record = await _uow.MedicalRecords.GetOrCreateAsync(patientId, clinicId, ct);
        }
        return _mapper.Map<MedicalRecordResponse>(record);
    }

    public async Task<MedicalRecordEntryResponse> AddEntryAsync(int clinicId, int doctorUserId, CreateMedicalRecordEntryRequest request, CancellationToken ct = default)
    {
        var record = await _uow.MedicalRecords.GetOrCreateAsync(request.PatientId, clinicId, ct);

        var entry = new MedicalRecordEntry
        {
            MedicalRecordId = record.Id,
            AppointmentId = request.AppointmentId,
            DoctorUserId = doctorUserId,
            EntryType = request.EntryType,
            Description = request.Description,
            IsConfidential = request.IsConfidential,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.MedicalRecords.AddEntryAsync(entry, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Entrada de prontuário criada: {EntryId}, Paciente: {PatientId}", entry.Id, request.PatientId);

        return _mapper.Map<MedicalRecordEntryResponse>(entry);
    }

    public async Task<MedicalRecordEntryResponse> GetEntryAsync(int entryId, CancellationToken ct = default)
    {
        var entry = await _uow.MedicalRecordEntries.GetByIdAsync(entryId, ct)
            ?? throw new NotFoundException("Entrada de prontuário", entryId);
        return _mapper.Map<MedicalRecordEntryResponse>(entry);
    }

    public async Task<ClinicalSummaryResponse> GetSummaryAsync(int patientId, int clinicId, CancellationToken ct = default)
    {
        var record = await _uow.MedicalRecords.GetByPatientIdWithEntriesAsync(patientId, clinicId, ct);

        if (record == null)
            return new ClinicalSummaryResponse("N/A", "Não informado", "Nenhuma", "Nenhum", 0);

        var summary = await _uow.MedicalRecords.GetClinicalSummaryAsync(record.Id, ct);
        var lastEntry = record.Entries.FirstOrDefault();

        return new ClinicalSummaryResponse(
            record.Patient?.Name ?? "N/A",
            record.Patient?.BloodType?.ToString() ?? "Não informado",
            record.Patient?.Allergies ?? "Nenhuma",
            lastEntry?.CreatedAt.ToString("dd/MM/yyyy") ?? "Nenhum",
            record.Entries.Count
        );
    }

    public async Task<MedicalRecordEntryResponse> UpdateEntryAsync(int entryId, int clinicId, UpdateMedicalRecordEntryRequest request, CancellationToken ct = default)
    {
        var entry = await _uow.MedicalRecordEntries.GetByIdAsync(entryId, ct)
            ?? throw new NotFoundException("Entrada de prontuário", entryId);

        if (request.Description != null)
            entry.Description = request.Description;
        if (request.IsConfidential.HasValue)
            entry.IsConfidential = request.IsConfidential.Value;

        await _uow.MedicalRecordEntries.UpdateAsync(entry, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<MedicalRecordEntryResponse>(entry);
    }
}