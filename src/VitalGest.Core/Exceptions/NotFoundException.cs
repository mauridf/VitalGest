namespace VitalGest.Core.Exceptions;

/// <summary>
/// Exceção lançada quando uma entidade não é encontrada.
/// Mapeada para HTTP 404 Not Found.
/// </summary>
public class NotFoundException : DomainException
{
    /// <summary>
    /// Nome da entidade que não foi encontrada.
    /// </summary>
    public string EntityName { get; }

    /// <summary>
    /// ID pesquisado.
    /// </summary>
    public object EntityId { get; }

    public NotFoundException(string entityName, object entityId)
        : base($"{entityName} com ID '{entityId}' não encontrado(a).", "NOT_FOUND")
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    public NotFoundException(string message)
        : base(message, "NOT_FOUND")
    {
        EntityName = string.Empty;
        EntityId = 0;
    }
}