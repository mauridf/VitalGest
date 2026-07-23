namespace VitalGest.Core.Enums;

/// <summary>
/// Tipo de documento armazenado no sistema.
/// </summary>
public enum DocumentType
{
    /// <summary>Receita/prescrição médica</summary>
    Prescription = 1,

    /// <summary>Atestado médico</summary>
    Atest = 2,

    /// <summary>Resultado de exame</summary>
    ExamResult = 3,

    /// <summary>Documento pessoal do paciente (RG, CPF, etc.)</summary>
    PersonalDocument = 4,

    /// <summary>Outros tipos de documento</summary>
    Other = 5
}