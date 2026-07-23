namespace VitalGest.Core.Enums;

/// <summary>
/// Métodos de pagamento aceitos pela clínica.
/// </summary>
public enum PaymentMethod
{
    /// <summary>Cartão de crédito</summary>
    CreditCard = 1,

    /// <summary>Cartão de débito</summary>
    DebitCard = 2,

    /// <summary>PIX</summary>
    Pix = 3,

    /// <summary>Dinheiro em espécie</summary>
    Cash = 4,

    /// <summary>Convênio/plano de saúde</summary>
    Insurance = 5,

    /// <summary>Transferência bancária</summary>
    BankTransfer = 6
}