using VitalGest.Core.Enums;

namespace VitalGest.Core.Entities;

/// <summary>
/// Notificação enviada a usuários ou pacientes.
/// Suporta lembretes de agendamento, confirmações, cancelamentos e notificações de resultados.
/// </summary>
public class Notification
{
    public int Id { get; set; }

    /// <summary>Clínica remetente da notificação (tenant)</summary>
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    /// <summary>Usuário destinatário (colaborador)</summary>
    public int? UserId { get; set; }
    public User? User { get; set; }

    /// <summary>Paciente destinatário</summary>
    public int? PatientId { get; set; }
    public Patient? Patient { get; set; }

    /// <summary>Título da notificação</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Mensagem/conteúdo da notificação</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Tipo de notificação (lembrete, confirmação, resultado, etc.)</summary>
    public NotificationType Type { get; set; }

    /// <summary>Canal de envio (in-app, email, sms)</summary>
    public string? Channel { get; set; }

    /// <summary>Data/hora do envio</summary>
    public DateTime? SentAt { get; set; }

    /// <summary>Notificação foi lida?</summary>
    public bool IsRead { get; set; }

    /// <summary>Data/hora da leitura</summary>
    public DateTime? ReadAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}