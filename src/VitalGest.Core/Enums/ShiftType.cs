namespace VitalGest.Core.Enums;

/// <summary>
/// Turno de trabalho do profissional.
/// </summary>
public enum ShiftType
{
    /// <summary>Turno da manhã</summary>
    Morning = 1,

    /// <summary>Turno da tarde</summary>
    Afternoon = 2,

    /// <summary>Turno da noite</summary>
    Night = 3,

    /// <summary>Turno integral (manhã + tarde)</summary>
    FullTime = 4
}