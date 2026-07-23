namespace VitalGest.Core.Enums;

/// <summary>
/// Status da fatura.
/// </summary>
public enum InvoiceStatus
{
    /// <summary>Fatura pendente de pagamento</summary>
    Pending = 1,

    /// <summary>Fatura paga integralmente</summary>
    Paid = 2,

    /// <summary>Fatura vencida</summary>
    Overdue = 3,

    /// <summary>Fatura cancelada</summary>
    Cancelled = 4
}