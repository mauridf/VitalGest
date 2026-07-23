namespace VitalGest.Core.Entities;

/// <summary>
/// Vínculo entre Usuário e Clínica (tabela de junção com dados adicionais).
/// Representa um colaborador/empregado de uma clínica.
/// </summary>
public class ClinicUser
{
    public int Id { get; set; }

    /// <summary>Usuário vinculado</summary>
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    /// <summary>Clínica onde trabalha (tenant)</summary>
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    /// <summary>Cargo do colaborador na clínica</summary>
    public int PositionId { get; set; }
    public Position Position { get; set; } = null!;

    /// <summary>Departamento onde trabalha (opcional)</summary>
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    /// <summary>Número do documento profissional (CRM, COREN, CRO, etc.)</summary>
    public string? ProfessionalDocument { get; set; }

    /// <summary>Tipo do documento profissional</summary>
    public string? ProfessionalDocumentType { get; set; }

    /// <summary>UF do documento profissional</summary>
    public string? ProfessionalDocumentUF { get; set; }

    /// <summary>Vínculo ativo?</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Data de contratação</summary>
    public DateTime? HireDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}