namespace VitalGest.Core.Entities;

/// <summary>
/// Resultado de exame laboratorial ou de imagem.
/// Contém o laudo, valores estruturados (JSON) e arquivos anexados.
/// </summary>
public class ExamResult
{
    public int Id { get; set; }

    /// <summary>Exame ao qual este resultado pertence</summary>
    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    /// <summary>Data/hora do resultado</summary>
    public DateTime ResultDate { get; set; } = DateTime.UtcNow;

    /// <summary>Resumo/laudo textual do resultado</summary>
    public string? Summary { get; set; }

    /// <summary>Valores estruturados do resultado em JSON (para exames com múltiplos parâmetros)</summary>
    public string? ResultJson { get; set; }

    /// <summary>URL do arquivo do resultado (PDF, imagem)</summary>
    public string? FileUrl { get; set; }

    /// <summary>Técnico que realizou o exame</summary>
    public int? PerformedById { get; set; }
    public User? PerformedBy { get; set; }

    /// <summary>Médico que revisou/validou o resultado</summary>
    public int? ReviewedById { get; set; }
    public User? ReviewedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}