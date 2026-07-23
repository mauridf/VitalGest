using VitalGest.Core.Enums;

namespace VitalGest.Core.Entities;

/// <summary>
/// Fatura/duplicata gerada para cobrança de serviços prestados.
/// Controla valores, vencimento e status de pagamento.
/// </summary>
public class Invoice
{
    public int Id { get; set; }

    /// <summary>Clínica emissora (tenant)</summary>
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    /// <summary>Paciente faturado (opcional para faturas genéricas)</summary>
    public int? PatientId { get; set; }
    public Patient? Patient { get; set; }

    /// <summary>Número único da fatura (por clínica)</summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>Data de emissão da fatura</summary>
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;

    /// <summary>Data de vencimento</summary>
    public DateOnly DueDate { get; set; }

    /// <summary>Valor total da fatura</summary>
    public decimal TotalAmount { get; set; }

    /// <summary>Valor já pago</summary>
    public decimal PaidAmount { get; set; }

    /// <summary>Status da fatura (pendente, paga, vencida, cancelada)</summary>
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;

    /// <summary>Observações sobre a fatura</summary>
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}