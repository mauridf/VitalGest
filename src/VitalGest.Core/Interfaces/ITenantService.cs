namespace VitalGest.Core.Interfaces;

/// <summary>
/// Serviço que gerencia o contexto multi-tenant.
/// Extrai e disponibiliza o ClinicId da requisição atual.
/// </summary>
public interface ITenantService
{
    /// <summary>
    /// ID da clínica (tenant) atual.
    /// Retorna null para requisições que não exigem tenant (auth, health).
    /// </summary>
    int? ClinicId { get; }

    /// <summary>
    /// Define o ClinicId a partir do token JWT ou header.
    /// </summary>
    void SetClinicId(int? clinicId);

    /// <summary>
    /// Verifica se o tenant atual é válido (ClinicId != null).
    /// </summary>
    bool HasTenant();
}