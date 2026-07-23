namespace VitalGest.Core.Entities;

/// <summary>
/// Item de uma prescrição médica.
/// Representa um medicamento com sua dosagem, frequência e duração.
/// </summary>
public class PrescriptionItem
{
    public int Id { get; set; }

    /// <summary>Prescrição à qual este item pertence</summary>
    public int PrescriptionId { get; set; }
    public Prescription Prescription { get; set; } = null!;

    /// <summary>Nome do medicamento prescrito</summary>
    public string MedicationName { get; set; } = string.Empty;

    /// <summary>Dosagem (ex: 500mg, 1 comprimido, 10ml)</summary>
    public string Dosage { get; set; } = string.Empty;

    /// <summary>Frequência de administração (ex: 8/8h, 1x ao dia)</summary>
    public string Frequency { get; set; } = string.Empty;

    /// <summary>Duração do tratamento (ex: 7 dias, 30 dias)</summary>
    public string? Duration { get; set; }

    /// <summary>Observações específicas sobre o medicamento</summary>
    public string? Notes { get; set; }

    /// <summary>Ordem de exibição na prescrição</summary>
    public int OrderNumber { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}