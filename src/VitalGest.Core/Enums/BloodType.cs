namespace VitalGest.Core.Enums;

/// <summary>
/// Tipo sanguíneo do paciente (sistema ABO + fator Rh).
/// </summary>
public enum BloodType
{
    /// <summary>A+</summary>
    APositive = 1,

    /// <summary>A-</summary>
    ANegative = 2,

    /// <summary>B+</summary>
    BPositive = 3,

    /// <summary>B-</summary>
    BNegative = 4,

    /// <summary>AB+</summary>
    ABPositive = 5,

    /// <summary>AB-</summary>
    ABNegative = 6,

    /// <summary>O+</summary>
    OPositive = 7,

    /// <summary>O-</summary>
    ONegative = 8,

    /// <summary>Não informado</summary>
    Unknown = 9
}