namespace VitalGest.Core.Enums;

/// <summary>
/// Tipo de entrada no prontuário eletrônico do paciente.
/// </summary>
public enum MedicalRecordEntryType
{
    /// <summary>Evolução clínica (anamnese, exame físico, diagnóstico)</summary>
    Evolution = 1,

    /// <summary>Registro de prescrição médica</summary>
    Prescription = 2,

    /// <summary>Solicitação de exame</summary>
    Exam = 3,

    /// <summary>Emissão de atestado</summary>
    Atest = 4,

    /// <summary>Receita médica</summary>
    Receipt = 5,

    /// <summary>Observação geral</summary>
    Observation = 6
}