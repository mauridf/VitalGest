using VitalGest.Core.Entities;

namespace VitalGest.Core.Interfaces;

/// <summary>
/// Repositório especializado para Clínicas.
/// </summary>
public interface IClinicRepository : IRepository<Clinic>
{
    /// <summary>Busca clínica por CNPJ</summary>
    Task<Clinic?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);

    /// <summary>Busca clínica com endereço e departamentos</summary>
    Task<Clinic?> GetByIdWithDetailsAsync(int clinicId, CancellationToken cancellationToken = default);

    /// <summary>Verifica se clínica está ativa</summary>
    Task<bool> IsActiveAsync(int clinicId, CancellationToken cancellationToken = default);
}