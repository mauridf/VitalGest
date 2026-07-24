namespace VitalGest.Application.DTOs.Audit;

public record AuditLogResponse(int Id, string EntityType, int EntityId, string Action, string? OldValues, string? NewValues, string? UserName, DateTime CreatedAt);