using VitalGest.Core.Enums;

namespace VitalGest.Core.Entities;

/// <summary>
/// Documento ou anexo armazenado no sistema (receitas, atestados, exames, documentos pessoais).
/// Pode estar associado a um paciente, agendamento ou exame.
/// </summary>
public class Document
{
    public int Id { get; set; }

    /// <summary>Clínica a que pertence o documento (tenant)</summary>
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    /// <summary>Paciente associado (opcional)</summary>
    public int? PatientId { get; set; }
    public Patient? Patient { get; set; }

    /// <summary>Agendamento associado (opcional)</summary>
    public int? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    /// <summary>Exame associado (opcional)</summary>
    public int? ExamId { get; set; }
    public Exam? Exam { get; set; }

    /// <summary>Nome original do arquivo</summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>URL ou caminho de armazenamento do arquivo</summary>
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>Tamanho do arquivo em bytes</summary>
    public long? FileSize { get; set; }

    /// <summary>Tipo de conteúdo (MIME type)</summary>
    public string? ContentType { get; set; }

    /// <summary>Tipo do documento (receita, atestado, exame, etc.)</summary>
    public DocumentType DocumentType { get; set; }

    /// <summary>Usuário que fez o upload</summary>
    public int? UploadedById { get; set; }
    public User? UploadedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}