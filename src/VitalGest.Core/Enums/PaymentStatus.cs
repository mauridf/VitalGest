namespace VitalGest.Core.Enums;

/// <summary>
/// Status do pagamento.
/// </summary>
public enum PaymentStatus
{
    /// <summary>Pagamento pendente</summary>
    Pending = 1,

    /// <summary>Pagamento realizado com sucesso</summary>
    Paid = 2,

    /// <summary>Pagamento cancelado/estornado</summary>
    Cancelled = 3,

    /// <summary>Pagamento parcial (parcelado ou desconto)</summary>
    Partial = 4
}