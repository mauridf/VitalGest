namespace VitalGest.Core.Entities;

/// <summary>
/// Tipo de exame médico disponível para solicitação.
/// Classifica exames em laboratoriais, imagem e outras categorias.
/// </summary>
public class ExamType
{
    public int Id { get; set; }

    /// <summary>Nome do exame (ex: Hemograma Completo, Raio-X Tórax)</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Descrição detalhada do exame</summary>
    public string? Description { get; set; }

    /// <summary>Categoria do exame (classificação interna)</summary>
    public int Category { get; set; }

    /// <summary>É um exame laboratorial (sangue, urina, etc.)?</summary>
    public bool IsLaboratory { get; set; }

    /// <summary>É um exame de imagem (raio-x, ultrassom, etc.)?</summary>
    public bool IsImage { get; set; }

    /// <summary>Exige preparação especial do paciente (jejum, etc.)?</summary>
    public bool RequiresPreparation { get; set; }

    /// <summary>Instruções de preparação para o paciente</summary>
    public string? PreparationInstructions { get; set; }

    /// <summary>Tipo de exame ativo para solicitação?</summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    public ICollection<InsuranceCoverage> Coverages { get; set; } = new List<InsuranceCoverage>();
}