using VitalGest.Core.Interfaces;

namespace VitalGest.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de tenant.
/// Armazena o ClinicId da requisição atual em um escopo thread-safe (AsyncLocal).
/// </summary>
public class TenantService : ITenantService
{
    private static readonly AsyncLocal<int?> _currentClinicId = new();

    /// <summary>
    /// ID da clínica (tenant) atual.
    /// Retorna null para requisições sem tenant (ex: auth, health check).
    /// </summary>
    public int? ClinicId
    {
        get => _currentClinicId.Value;
        private set => _currentClinicId.Value = value;
    }

    /// <summary>
    /// Define o ClinicId a partir do token JWT.
    /// Chamado pelo TenantResolutionMiddleware.
    /// </summary>
    public void SetClinicId(int? clinicId)
    {
        ClinicId = clinicId;
    }

    /// <summary>
    /// Verifica se há um tenant definido para a requisição atual.
    /// </summary>
    public bool HasTenant() => ClinicId.HasValue;
}