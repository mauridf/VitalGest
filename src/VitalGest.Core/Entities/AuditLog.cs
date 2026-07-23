namespace VitalGest.Core.Entities;

/// <summary>
/// Log de auditoria que registra todas as operações de criação, alteração e exclusão.
/// Armazena valores anteriores e novos para rastreabilidade completa.
/// </summary>
public class AuditLog
{
    public int Id { get; set; }

    /// <summary>Clínica onde a operação ocorreu (pode ser nulo para operações globais)</summary>
    public int? ClinicId { get; set; }
    public Clinic? Clinic { get; set; }

    /// <summary>Usuário que realizou a operação</summary>
    public int? UserId { get; set; }
    public User? User { get; set; }

    /// <summary>Nome da entidade afetada</summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>ID da entidade afetada</summary>
    public int EntityId { get; set; }

    /// <summary>Ação realizada (Create, Update, Delete)</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>Valores anteriores (JSON) para operações de update/delete</summary>
    public string? OldValues { get; set; }

    /// <summary>Novos valores (JSON) para operações de create/update</summary>
    public string? NewValues { get; set; }

    /// <summary>Endereço IP do solicitante</summary>
    public string? IpAddress { get; set; }

    /// <summary>User Agent do navegador/cliente</summary>
    public string? UserAgent { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}