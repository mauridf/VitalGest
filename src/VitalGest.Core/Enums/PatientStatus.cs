namespace VitalGest.Core.Enums;

/// <summary>
/// Status do paciente no sistema.
/// </summary>
public enum PatientStatus
{
    /// <summary>Paciente ativo, em acompanhamento</summary>
    Active = 1,

    /// <summary>Paciente inativo/sem movimentação recente</summary>
    Inactive = 2,

    /// <summary>Paciente suspenso/bloqueado</summary>
    Suspended = 3,

    /// <summary>Paciente falecido</summary>
    Deceased = 4
}
