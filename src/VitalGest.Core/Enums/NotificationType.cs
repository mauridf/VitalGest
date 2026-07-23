namespace VitalGest.Core.Enums;

/// <summary>
/// Tipo de notificação enviada ao usuário/paciente.
/// </summary>
public enum NotificationType
{
    /// <summary>Lembrete de agendamento (24h ou 1h antes)</summary>
    AppointmentReminder = 1,

    /// <summary>Confirmação de agendamento</summary>
    AppointmentConfirmed = 2,

    /// <summary>Cancelamento de agendamento</summary>
    AppointmentCancelled = 3,

    /// <summary>Resultado de exame disponível</summary>
    ExamResultReady = 4,

    /// <summary>Notificação geral/administrativa</summary>
    General = 5
}