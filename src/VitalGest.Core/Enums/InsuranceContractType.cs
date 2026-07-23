namespace VitalGest.Core.Enums;

/// <summary>
/// Tipo de contrato do plano de saúde.
/// </summary>
public enum InsuranceContractType
{
    /// <summary>Plano público (SUS, servidores)</summary>
    Public = 1,

    /// <summary>Plano privado/pessoa física</summary>
    Private = 2,

    /// <summary>Plano empresarial/corporativo</summary>
    Corporate = 3
}