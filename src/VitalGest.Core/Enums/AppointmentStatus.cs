namespace VitalGest.Core.Enums;

/// <summary>
/// Status do agendamento de consulta/exame/procedimento.
/// </summary>
public enum AppointmentStatus
{
    /// <summary>Agendamento criado, aguardando confirmação</summary>
    Scheduled = 1,

    /// <summary>Agendamento confirmado pelo paciente ou clínica</summary>
    Confirmed = 2,

    /// <summary>Paciente em atendimento</summary>
    InProgress = 3,

    /// <summary>Atendimento concluído</summary>
    Completed = 4,

    /// <summary>Agendamento cancelado</summary>
    Cancelled = 5,

    /// <summary>Paciente não compareceu</summary>
    NoShow = 6
}