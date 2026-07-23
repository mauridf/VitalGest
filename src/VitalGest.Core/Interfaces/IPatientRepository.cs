using VitalGest.Core.Entities;

namespace VitalGest.Core.Interfaces;

/// <summary>
/// Repositório especializado para Pacientes.
/// </summary>
public interface IPatientRepository : IRepository<Patient>
{
    /// <summary>Busca paciente por CPF (global, ignora tenant filter)</summary>
    Task<Patient?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);

    /// <summary>Busca texto em nome do paciente (case-insensitive, usa pg_trgm)</summary>
    Task<IEnumerable<Patient>> SearchByNameAsync(string query, int clinicId, CancellationToken cancellationToken = default);

    /// <summary>Busca pacientes por nome parcial ou CPF</summary>
    Task<IEnumerable<Patient>> SearchAsync(string query, int clinicId, int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>Lista pacientes com convênio</summary>
    Task<IEnumerable<Patient>> GetByInsurancePlanIdAsync(int insurancePlanId, int clinicId, CancellationToken cancellationToken = default);

    /// <summary>Obtém paciente com todos os relacionamentos (convênio, endereço)</summary>
    Task<Patient?> GetByIdWithDetailsAsync(int patientId, int clinicId, CancellationToken cancellationToken = default);
}