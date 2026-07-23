namespace VitalGest.Core.Enums;

/// <summary>
/// Tipo de agendamento.
/// </summary>
public enum AppointmentType
{
    /// <summary>Consulta médica/odontológica</summary>
    Consultation = 1,

    /// <summary>Exame laboratorial ou de imagem</summary>
    Exam = 2,

    /// <summary>Retorno de consulta</summary>
    Return = 3,

    /// <summary>Procedimento médico/odontológico</summary>
    Procedure = 4
}