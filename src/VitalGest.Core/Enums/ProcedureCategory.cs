namespace VitalGest.Core.Enums;

/// <summary>
/// Categoria do procedimento médico/odontológico/laboratorial.
/// </summary>
public enum ProcedureCategory
{
    /// <summary>Consulta médica ou odontológica</summary>
    Consultation = 1,

    /// <summary>Exame laboratorial</summary>
    LaboratoryExam = 2,

    /// <summary>Exame de imagem</summary>
    ImageExam = 3,

    /// <summary>Procedimento cirúrgico</summary>
    Surgery = 4,

    /// <summary>Procedimento odontológico</summary>
    DentalProcedure = 5,

    /// <summary>Outros tipos de procedimento</summary>
    Other = 6
}
