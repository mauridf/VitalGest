using VitalGest.Core.Enums;

namespace VitalGest.Core.Entities;

/// <summary>
/// Pagamento recebido por consulta, exame ou procedimento.
/// Controla valores, descontos, método de pagamento e parcelamento.
/// </summary>
public class Payment
{
    public int Id { get; set; }

    /// <summary>Clínica que recebeu o pagamento (tenant)</summary>
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    /// <summary>Paciente que realizou o pagamento</summary>
    public int? PatientId { get; set; }
    public Patient? Patient { get; set; }

    /// <summary>Agendamento associado ao pagamento (opcional)</summary>
    public int? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    /// <summary>Valor bruto do pagamento (sem descontos)</summary>
    public decimal Amount { get; set; }

    /// <summary>Valor do desconto concedido</summary>
    public decimal Discount { get; set; }

    /// <summary>Valor total após descontos (Amount - Discount)</summary>
    public decimal TotalAmount { get; set; }

    /// <summary>Data/hora do pagamento</summary>
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    /// <summary>Método de pagamento (crédito, débito, PIX, dinheiro, convênio, transferência)</summary>
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>Status do pagamento (pendente, pago, cancelado, parcial)</summary>
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    /// <summary>Número de parcelas (1 = à vista)</summary>
    public int Installments { get; set; } = 1;

    /// <summary>Observações sobre o pagamento</summary>
    public string? Notes { get; set; }

    /// <summary>Usuário que recebeu o pagamento</summary>
    public int? ReceivedById { get; set; }
    public User? ReceivedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}