using VitalGest.Core.Entities;

namespace VitalGest.Core.Interfaces;

/// <summary>
/// Repositório especializado para Prontuários.
/// </summary>
public interface IMedicalRecordRepository : IRepository<MedicalRecord>
{
    /// <summary>Busca prontuário do paciente com todas as entradas</summary>
    Task<MedicalRecord?> GetByPatientIdWithEntriesAsync(int patientId, int clinicId, CancellationToken cancellationToken = default);

    /// <summary>Busca ou cria prontuário para paciente</summary>
    Task<MedicalRecord> GetOrCreateAsync(int patientId, int clinicId, CancellationToken cancellationToken = default);

    /// <summary>Adiciona entrada no prontuário</summary>
    Task<MedicalRecordEntry> AddEntryAsync(MedicalRecordEntry entry, CancellationToken cancellationToken = default);

    /// <summary>Lista entradas do prontuário em ordem cronológica</summary>
    Task<IEnumerable<MedicalRecordEntry>> GetEntriesAsync(int medicalRecordId, CancellationToken cancellationToken = default);

    /// <summary>Gera resumo clínico do paciente</summary>
    Task<string> GetClinicalSummaryAsync(int medicalRecordId, CancellationToken cancellationToken = default);
}