namespace VitalGest.Core.Enums;

/// <summary>
/// Status do exame no fluxo laboratorial.
/// </summary>
public enum ExamStatus
{
    /// <summary>Exame solicitado pelo médico</summary>
    Requested = 1,

    /// <summary>Amostra coletada</summary>
    Collected = 2,

    /// <summary>Em análise pelo laboratório</summary>
    InAnalysis = 3,

    /// <summary>Resultado pronto para liberação</summary>
    Ready = 4,

    /// <summary>Resultado entregue ao paciente/médico</summary>
    Delivered = 5
}