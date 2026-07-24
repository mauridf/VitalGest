using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Notifications;
using VitalGest.Application.Services;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.UnitTests.Services;

/// <summary>
/// Testes unitários para o NotificationService.
/// </summary>
public class NotificationServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<NotificationService> _logger;
    private readonly NotificationService _sut;

    public NotificationServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<NotificationService>>();
        _sut = new NotificationService(_uow, _mapper, _logger);
    }

    [Fact]
    public async Task MarkAsRead_WithInvalidId_ShouldThrowNotFoundException()
    {
        _uow.Notifications.GetByIdAsync(999).Returns((Notification?)null);

        var act = () => _sut.MarkAsReadAsync(999);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Notificação*");
    }

    [Fact]
    public async Task MarkAsRead_WithValidId_ShouldSetReadTimestamp()
    {
        var notification = new Notification { Id = 1, IsRead = false };
        _uow.Notifications.GetByIdAsync(1).Returns(notification);

        await _sut.MarkAsReadAsync(1);

        notification.IsRead.Should().BeTrue();
        notification.ReadAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        await _uow.Notifications.Received(1).UpdateAsync(notification);
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GetUnreadCount_ShouldReturnCount()
    {
        _uow.Notifications.CountAsync(Arg.Any<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>()).Returns(5);

        var result = await _sut.GetUnreadCountAsync(1, 1);

        result.Count.Should().Be(5);
    }

    [Fact]
    public async Task Send_ShouldCreateNotification()
    {
        var request = new SendNotificationRequest("Lembrete", "Sua consulta é amanhã", NotificationType.AppointmentReminder, UserId: 1);

        await _sut.SendAsync(1, request);

        await _uow.Notifications.Received(1).AddAsync(Arg.Is<Notification>(n => n.Title == "Lembrete" && n.ClinicId == 1));
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GetByUser_ShouldReturnOrderedNotifications()
    {
        var notifications = new List<Notification>
        {
            new() { Id = 1, UserId = 1, ClinicId = 1, CreatedAt = DateTime.UtcNow.AddMinutes(-5) },
            new() { Id = 2, UserId = 1, ClinicId = 1, CreatedAt = DateTime.UtcNow }
        };
        var responses = new[]
        {
            new NotificationResponse(2, "Title2", "Message2", "Info", false, DateTime.UtcNow, DateTime.UtcNow),
            new NotificationResponse(1, "Title1", "Message1", "Info", false, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(-5))
        };

        _uow.Notifications.FindAsync(Arg.Any<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>()).Returns(notifications.OrderByDescending(n => n.CreatedAt).ToList());
        _mapper.Map<IEnumerable<NotificationResponse>>(Arg.Any<IEnumerable<Notification>>()).Returns(responses);

        var result = await _sut.GetByUserAsync(1, 1);

        result.Should().HaveCount(2);
        result.First().Message.Should().Be("Message2");
    }
}
