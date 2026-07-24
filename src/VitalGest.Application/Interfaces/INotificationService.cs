using VitalGest.Application.DTOs.Notifications;

namespace VitalGest.Application.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationResponse>> GetByUserAsync(int userId, int clinicId, CancellationToken ct = default);
    Task MarkAsReadAsync(int id, CancellationToken ct = default);
    Task<UnreadCountResponse> GetUnreadCountAsync(int userId, int clinicId, CancellationToken ct = default);
    Task SendAsync(int clinicId, SendNotificationRequest request, CancellationToken ct = default);
}