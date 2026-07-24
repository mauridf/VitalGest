using VitalGest.Core.Enums;

namespace VitalGest.Application.DTOs.Notifications;

public record SendNotificationRequest(string Title, string Message, NotificationType Type, int? UserId = null, int? PatientId = null, string? Channel = null);
public record NotificationResponse(int Id, string Title, string Message, string Type, bool IsRead, DateTime? SentAt, DateTime CreatedAt);
public record UnreadCountResponse(int Count);