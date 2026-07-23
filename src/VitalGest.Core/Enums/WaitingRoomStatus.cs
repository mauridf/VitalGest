namespace VitalGest.Core.Enums;

/// <summary>
/// Status do paciente na sala de espera.
/// </summary>
public enum WaitingRoomStatus
{
    /// <summary>Aguardando ser chamado</summary>
    Waiting = 1,

    /// <summary>Chamado para atendimento</summary>
    Called = 2,

    /// <summary>Em atendimento</summary>
    InProgress = 3,

    /// <summary>Atendimento finalizado</summary>
    Finished = 4
}