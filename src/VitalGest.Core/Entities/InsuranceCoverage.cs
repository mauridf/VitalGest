namespace VitalGest.Core.Entities;

/// <summary>
/// Cobertura de um plano de saúde para determinado procedimento, exame ou especialidade.
/// Define o percentual coberto, necessidade de autorização e limite de sessões.
/// </summary>
public class InsuranceCoverage
{
    public int Id { get; set; }

    /// <summary>Plano de saúde ao qual esta cobertura pertence</summary>
    public int InsurancePlanId { get; set; }
    public InsurancePlan InsurancePlan { get; set; } = null!;

    /// <summary>Tipo de exame coberto (opcional — cobertura específica por exame)</summary>
    public int? ExamTypeId { get; set; }
    public ExamType? ExamType { get; set; }

    /// <summary>Especialidade coberta (opcional — cobertura por especialidade)</summary>
    public int? SpecialtyId { get; set; }
    public Specialty? Specialty { get; set; }

    /// <summary>Tipo de procedimento coberto (opcional)</summary>
    public int? ProcedureType { get; set; }

    /// <summary>Percentual de cobertura (ex: 80.00 = 80%)</summary>
    public decimal CoveragePercent { get; set; } = 100.00m;

    /// <summary>Exige autorização prévia do convênio?</summary>
    public bool RequiresAuthorization { get; set; }

    /// <summary>Número máximo de sessões autorizadas</summary>
    public int? MaxSessions { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}