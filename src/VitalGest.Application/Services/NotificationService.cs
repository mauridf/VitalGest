using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Notifications;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Entities;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IUnitOfWork uow, IMapper mapper, ILogger<NotificationService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<NotificationResponse>> GetByUserAsync(int userId, int clinicId, CancellationToken ct = default)
    {
        var notifications = await _uow.Notifications.FindAsync(
            n => n.UserId == userId && n.ClinicId == clinicId, ct);

        return _mapper.Map<IEnumerable<NotificationResponse>>(
            notifications.OrderByDescending(n => n.CreatedAt));
    }

    public async Task MarkAsReadAsync(int id, CancellationToken ct = default)
    {
        var notification = await _uow.Notifications.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Notificação", id);

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;

        await _uow.Notifications.UpdateAsync(notification, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<UnreadCountResponse> GetUnreadCountAsync(int userId, int clinicId, CancellationToken ct = default)
    {
        var count = await _uow.Notifications.CountAsync(
            n => n.UserId == userId && n.ClinicId == clinicId && !n.IsRead, ct);

        return new UnreadCountResponse(count);
    }

    public async Task SendAsync(int clinicId, SendNotificationRequest request, CancellationToken ct = default)
    {
        var notification = new Notification
        {
            ClinicId = clinicId,
            UserId = request.UserId,
            PatientId = request.PatientId,
            Title = request.Title,
            Message = request.Message,
            Type = request.Type,
            Channel = request.Channel ?? "in-app",
            SentAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Notifications.AddAsync(notification, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Notificação enviada: {NotifId}, Tipo: {Type}", notification.Id, request.Type);
    }
}