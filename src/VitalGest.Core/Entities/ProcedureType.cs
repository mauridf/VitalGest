using VitalGest.Core.Enums;

namespace VitalGest.Core.Entities;

/// <summary>
/// Catálogo de tipos de procedimentos médicos/odontológicos/laboratoriais.
/// Define os procedimentos disponíveis para agendamento, faturamento e cobertura de convênios.
/// </summary>
public class ProcedureType
{
    public int Id { get; set; }

    /// <summary>Nome do procedimento (ex: Hemograma, Limpeza Dentária, Raio-X Tórax)</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Descrição detalhada do procedimento</summary>
    public string? Description { get; set; }

    /// <summary>Categoria do procedimento (consulta, exame, cirurgia, etc.)</summary>
    public ProcedureCategory Category { get; set; }

    /// <summary>Código TUSS (padrão ANS) para procedimentos</summary>
    public string? TussCode { get; set; }

    /// <summary>Valor padrão cobrado pelo procedimento</summary>
    public decimal? DefaultPrice { get; set; }

    /// <summary>Duração padrão em minutos para agendamento</summary>
    public int? DefaultDuration { get; set; }

    /// <summary>Exige autorização prévia do convênio?</summary>
    public bool RequiresAuthorization { get; set; }

    /// <summary>Procedimento ativo para agendamento/faturamento</summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
