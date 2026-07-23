using VitalGest.Core.Enums;

namespace VitalGest.Core.Entities;

/// <summary>
/// Plano de saúde / convênio médico.
/// Define as operadoras e planos disponíveis para associação aos pacientes.
/// </summary>
public class InsurancePlan
{
    public int Id { get; set; }

    /// <summary>Nome do plano de saúde / operadora</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>CNPJ da operadora</summary>
    public string? CNPJ { get; set; }

    /// <summary>Telefone de contato da operadora</summary>
    public string? Phone { get; set; }

    /// <summary>E-mail de contato da operadora</summary>
    public string? Email { get; set; }

    /// <summary>Tipo de contrato (público, privado, empresarial)</summary>
    public InsuranceContractType ContractType { get; set; } = InsuranceContractType.Private;

    /// <summary>Plano ativo para associação?</summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Relacionamentos
    public ICollection<InsuranceCoverage> Coverages { get; set; } = new List<InsuranceCoverage>();
    public ICollection<Patient> Patients { get; set; } = new List<Patient>();
}