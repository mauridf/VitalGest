using VitalGest.Core.Entities;
using VitalGest.Core.Enums;

namespace VitalGest.Core.Interfaces;

/// <summary>
/// Repositório especializado para Exames.
/// </summary>
public interface IExamRepository : IRepository<Exam>
{
    /// <summary>Busca exames do paciente</summary>
    Task<IEnumerable<Exam>> GetByPatientIdAsync(int patientId, int clinicId, CancellationToken cancellationToken = default);

    /// <summary>Busca exames por status</summary>
    Task<IEnumerable<Exam>> GetByStatusAsync(ExamStatus status, int clinicId, CancellationToken cancellationToken = default);

    /// <summary>Busca exame com resultado</summary>
    Task<Exam?> GetByIdWithResultAsync(int examId, int clinicId, CancellationToken cancellationToken = default);

    /// <summary>Registra resultado do exame</summary>
    Task<ExamResult> AddResultAsync(ExamResult result, CancellationToken cancellationToken = default);

    /// <summary>Lista exames pendentes de resultado</summary>
    Task<IEnumerable<Exam>> GetPendingResultsAsync(int clinicId, CancellationToken cancellationToken = default);
}