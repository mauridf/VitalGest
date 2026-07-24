using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Appointments;
using VitalGest.Application.Services;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.UnitTests.Services;

/// <summary>
/// Testes unitários para o AppointmentService.
/// Foco em regras de negócio: conflitos, transições de status, validações.
/// </summary>
public class AppointmentServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<AppointmentService> _logger;
    private readonly AppointmentService _sut;

    public AppointmentServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<AppointmentService>>();
        _sut = new AppointmentService(_uow, _mapper, _logger);
    }

    [Fact]
    public async Task Create_WithTimeConflict_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var request = new CreateAppointmentRequest(
            1, // PatientId
            2, // DoctorUserId
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), // Amanhã
            new TimeOnly(14, 0),
            new TimeOnly(14, 30)
        );

        _uow.Appointments.HasTimeConflictAsync(
            Arg.Is<int>(2), // DoctorUserId
            Arg.Any<DateOnly>(),
            Arg.Any<TimeOnly>(),
            Arg.Any<TimeOnly>(),
            null)
            .Returns(true); // Conflito detectado!

        // Act
        var act = () => _sut.CreateAsync(1, 3, request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*já possui um agendamento neste horário*");
    }

    [Fact]
    public async Task Create_WithPastDate_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var request = new CreateAppointmentRequest(
            1, 2,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), // Ontem
            new TimeOnly(14, 0),
            new TimeOnly(14, 30)
        );

        // Act
        var act = () => _sut.CreateAsync(1, 3, request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Não é possível agendar para uma data passada*");
    }

    [Fact]
    public async Task Cancel_WithEmptyReason_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        _uow.Appointments.GetByIdAsync(1).Returns(new Appointment
        {
            Id = 1,
            ClinicId = 1,
            AppointmentDate = tomorrow,
            Status = AppointmentStatus.Scheduled
        });

        // Act
        var act = () => _sut.CancelAsync(1, 1, "");

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Motivo do cancelamento é obrigatório*");
    }

    [Fact]
    public async Task UpdateStatus_FromScheduledToCompleted_ShouldThrowBusinessRuleException()
    {
        // Arrange
        _uow.Appointments.GetByIdAsync(1).Returns(new Appointment
        {
            Id = 1,
            ClinicId = 1,
            Status = AppointmentStatus.Scheduled
        });

        // Scheduled → Completed não é permitido (deve passar por InProgress)
        // Act
        var act = () => _sut.UpdateStatusAsync(1, 1,
            new UpdateAppointmentStatusRequest(AppointmentStatus.Completed));

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Não é permitido transitar*");
    }

    [Fact]
    public async Task MarkNoShow_WithAlreadyCancelled_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var appointment = new Appointment
        {
            Id = 1,
            ClinicId = 1,
            Status = AppointmentStatus.Cancelled
        };

        _uow.Appointments.GetByIdAsync(1).Returns(appointment);

        // Act
        var act = () => _sut.MarkNoShowAsync(1, 1);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Status inválido*");
    }

    [Fact]
    public async Task Create_WithValidData_ShouldCreateAppointment()
    {
        // Arrange
        var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var request = new CreateAppointmentRequest(
            1, // PatientId
            2, // DoctorUserId
            tomorrow,
            new TimeOnly(14, 0),
            new TimeOnly(14, 30)
        );

        _uow.Appointments.HasTimeConflictAsync(
            Arg.Is<int>(2),
            Arg.Any<DateOnly>(),
            Arg.Any<TimeOnly>(),
            Arg.Any<TimeOnly>(),
            null)
            .Returns(false);

        _uow.Patients.GetByIdAsync(1).Returns(new Patient { Id = 1, ClinicId = 1, Name = "John" });

        var appointment = new Appointment { Id = 1, Status = AppointmentStatus.Scheduled };
        _mapper.Map<Appointment>(request).Returns(appointment);
        _uow.Appointments.AddAsync(Arg.Any<Appointment>()).Returns(appointment);
        _mapper.Map<AppointmentResponse>(Arg.Any<Appointment>())
            .Returns(new AppointmentResponse(1, 1, "John", 2, "Dr. House", tomorrow, new TimeOnly(14, 0), new TimeOnly(14, 30), AppointmentStatus.Scheduled, "Scheduled", AppointmentType.Consultation, "Consultation", null, null, false, null, DateTime.UtcNow));

        // Act
        var result = await _sut.CreateAsync(1, 3, request);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(AppointmentStatus.Scheduled);
        result.PatientName.Should().Be("John");
        await _uow.Appointments.Received(1).AddAsync(Arg.Any<Appointment>());
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Create_WithNonExistentPatient_ShouldThrowNotFoundException()
    {
        // Arrange
        var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var request = new CreateAppointmentRequest(1, 2, tomorrow, new TimeOnly(14, 0), new TimeOnly(14, 30));

        _uow.Appointments.HasTimeConflictAsync(Arg.Any<int>(), Arg.Any<DateOnly>(), Arg.Any<TimeOnly>(), Arg.Any<TimeOnly>(), null).Returns(false);
        _uow.Patients.GetByIdAsync(1).Returns((Patient?)null);

        // Act
        var act = () => _sut.CreateAsync(1, 3, request);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Paciente*");
    }

    [Fact]
    public async Task Confirm_WithScheduledStatus_ShouldSetConfirmed()
    {
        // Arrange
        var appointment = new Appointment
        {
            Id = 1,
            ClinicId = 1,
            Status = AppointmentStatus.Scheduled
        };

        _uow.Appointments.GetByIdAsync(1).Returns(appointment);

        // Act
        var result = await _sut.ConfirmAsync(1, 1);

        // Assert
        appointment.Status.Should().Be(AppointmentStatus.Confirmed);
        appointment.IsConfirmed.Should().BeTrue();
        await _uow.Appointments.Received(1).UpdateAsync(appointment);
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Confirm_WithNonScheduledStatus_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var appointment = new Appointment
        {
            Id = 1,
            ClinicId = 1,
            Status = AppointmentStatus.Cancelled
        };

        _uow.Appointments.GetByIdAsync(1).Returns(appointment);

        // Act
        var act = () => _sut.ConfirmAsync(1, 1);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Apenas agendamentos com status 'Agendado' podem ser confirmados*");
    }

    [Fact]
    public async Task Cancel_WithAlreadyCancelled_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var appointment = new Appointment
        {
            Id = 1,
            ClinicId = 1,
            AppointmentDate = tomorrow,
            Status = AppointmentStatus.Cancelled
        };

        _uow.Appointments.GetByIdAsync(1).Returns(appointment);

        // Act
        var act = () => _sut.CancelAsync(1, 1, "Paciente desistiu");

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Agendamento já está cancelado*");
    }

    [Fact]
    public async Task Cancel_WithPastDate_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var appointment = new Appointment
        {
            Id = 1,
            ClinicId = 1,
            AppointmentDate = yesterday,
            Status = AppointmentStatus.Scheduled
        };

        _uow.Appointments.GetByIdAsync(1).Returns(appointment);

        // Act
        var act = () => _sut.CancelAsync(1, 1, "Motivo");

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Não é possível cancelar agendamentos passados*");
    }

    [Fact]
    public async Task MarkNoShow_WithScheduledStatus_ShouldSetNoShow()
    {
        // Arrange
        var appointment = new Appointment
        {
            Id = 1,
            ClinicId = 1,
            Status = AppointmentStatus.Scheduled
        };

        _uow.Appointments.GetByIdAsync(1).Returns(appointment);

        // Act
        var result = await _sut.MarkNoShowAsync(1, 1);

        // Assert
        appointment.Status.Should().Be(AppointmentStatus.NoShow);
        await _uow.Appointments.Received(1).UpdateAsync(appointment);
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GetByDate_ShouldReturnAppointments()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var appointments = new List<Appointment> { new() { Id = 1, ClinicId = 1 } };
        var responses = new[] { new AppointmentResponse(1, 1, "John", 2, "Dr. House", date, new TimeOnly(14, 0), new TimeOnly(14, 30), AppointmentStatus.Scheduled, "Scheduled", AppointmentType.Consultation, "Consultation", null, null, false, null, DateTime.UtcNow) };

        _uow.Appointments.GetByDateAsync(date, 1).Returns(appointments);
        _mapper.Map<IEnumerable<AppointmentResponse>>(appointments).Returns(responses);

        // Act
        var result = await _sut.GetByDateAsync(1, date);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task Update_WithWrongClinic_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var request = new UpdateAppointmentRequest(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), new TimeOnly(10, 0), new TimeOnly(10, 30));
        var appointment = new Appointment { Id = 1, ClinicId = 2, AppointmentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)) };

        _uow.Appointments.GetByIdAsync(1).Returns(appointment);

        // Act
        var act = () => _sut.UpdateAsync(1, 1, request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Agendamento não pertence a esta clínica*");
    }
}