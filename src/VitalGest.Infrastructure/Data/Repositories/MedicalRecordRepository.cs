using Microsoft.EntityFrameworkCore;
using VitalGest.Core.Entities;
using VitalGest.Core.Interfaces;
using VitalGest.Infrastructure.Data.Context;

namespace VitalGest.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório especializado para Prontuários Eletrônicos.
/// </summary>
public class MedicalRecordRepository : Repository<MedicalRecord>, IMedicalRecordRepository
{
    public MedicalRecordRepository(VitalGestDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<MedicalRecord?> GetByPatientIdWithEntriesAsync(
        int patientId,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(mr => mr.Entries.OrderByDescending(e => e.CreatedAt))
                .ThenInclude(e => e.Doctor)
            .AsNoTracking()
            .FirstOrDefaultAsync(mr => mr.PatientId == patientId && mr.ClinicId == clinicId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<MedicalRecord> GetOrCreateAsync(
        int patientId,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        var record = await _dbSet
            .FirstOrDefaultAsync(mr => mr.PatientId == patientId && mr.ClinicId == clinicId, cancellationToken);

        if (record == null)
        {
            record = new MedicalRecord
            {
                PatientId = patientId,
                ClinicId = clinicId,
                CreatedAt = DateTime.UtcNow
            };
            await _dbSet.AddAsync(record, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return record;
    }

    /// <inheritdoc />
    public async Task<MedicalRecordEntry> AddEntryAsync(
        MedicalRecordEntry entry,
        CancellationToken cancellationToken = default)
    {
        await _context.MedicalRecordEntries.AddAsync(entry, cancellationToken);
        return entry;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MedicalRecordEntry>> GetEntriesAsync(
        int medicalRecordId,
        CancellationToken cancellationToken = default)
    {
        return await _context.MedicalRecordEntries
            .Include(e => e.Doctor)
            .Where(e => e.MedicalRecordId == medicalRecordId)
            .OrderByDescending(e => e.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> GetClinicalSummaryAsync(
        int medicalRecordId,
        CancellationToken cancellationToken = default)
    {
        var record = await _dbSet
            .Include(mr => mr.Patient)
            .Include(mr => mr.Entries.OrderByDescending(e => e.CreatedAt).Take(10))
            .AsNoTracking()
            .FirstOrDefaultAsync(mr => mr.Id == medicalRecordId, cancellationToken);

        if (record == null || record.Entries.Count == 0)
            return "Nenhum registro clínico encontrado.";

        var patient = record.Patient;
        var lastEntry = record.Entries.First();

        return $"Paciente: {patient?.Name ?? "N/A"}\n" +
               $"Tipo Sanguíneo: {patient?.BloodType?.ToString() ?? "Não informado"}\n" +
               $"Alergias: {patient?.Allergies ?? "Nenhuma registrada"}\n" +
               $"Último atendimento: {lastEntry.CreatedAt:dd/MM/yyyy HH:mm}\n" +
               $"Total de entradas: {record.Entries.Count}";
    }
}