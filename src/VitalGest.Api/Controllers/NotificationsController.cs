using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Application.DTOs.Notifications;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de notificações do usuário.
/// </summary>
[Authorize]
public class NotificationsController : BaseApiController
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Lista notificações do usuário logado.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NotificationResponse>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var userId = GetUserId();
        var result = await _notificationService.GetByUserAsync(userId, clinicId);
        return OkResponse(result);
    }

    /// <summary>
    /// Marca uma notificação como lida.
    /// </summary>
    [HttpPatch("{id:int}/read")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return OkResponse(new { }, "Notificação marcada como lida.");
    }

    /// <summary>
    /// Envia uma notificação (admin).
    /// </summary>
    [HttpPost("send")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Send([FromBody] SendNotificationRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        await _notificationService.SendAsync(clinicId, request);
        return OkResponse(new { }, "Notificação enviada com sucesso.");
    }

    /// <summary>
    /// Obtém a contagem de notificações não lidas.
    /// </summary>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(UnreadCountResponse), 200)]
    public async Task<IActionResult> GetUnreadCount()
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var userId = GetUserId();
        var result = await _notificationService.GetUnreadCountAsync(userId, clinicId);
        return OkResponse(result);
    }
}