using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Schedule;
using VitalGest.Application.Services;
using VitalGest.Core.Entities;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.UnitTests.Services;

/// <summary>
/// Testes unitários para o ScheduleService.
/// </summary>
public class ScheduleServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<ScheduleService> _logger;
    private readonly ScheduleService _sut;

    public ScheduleServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<ScheduleService>>();
        _sut = new ScheduleService(_uow, _mapper, _logger);
    }

    [Fact]
    public async Task Create_WithInvalidDayOfWeek_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var request = new CreateScheduleRequest(1, 7, // 7 é inválido (0-6)
            new TimeOnly(8, 0), new TimeOnly(18, 0));

        // Act
        var act = () => _sut.CreateAsync(1, request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Dia da semana inválido*");
    }

    [Fact]
    public async Task Create_WithStartTimeAfterEndTime_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var request = new CreateScheduleRequest(1, 1,
            new TimeOnly(18, 0),  // Início
            new TimeOnly(8, 0));  // Fim (antes do início!)

        // Act
        var act = () => _sut.CreateAsync(1, request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Horário inicial deve ser anterior*");
    }

    [Fact]
    public async Task Create_WithInvalidSlotDuration_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var request = new CreateScheduleRequest(1, 1,
            new TimeOnly(8, 0), new TimeOnly(18, 0), 10); // 10 min (mínimo 15)

        // Act
        var act = () => _sut.CreateAsync(1, request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Duração do slot deve ser entre 15 e 120 minutos*");
    }

    [Fact]
    public async Task Create_WithValidData_ShouldCreateSchedule()
    {
        // Arrange
        var request = new CreateScheduleRequest(1, 1,
            new TimeOnly(8, 0), new TimeOnly(18, 0), 30);
        var schedule = new Schedule { Id = 1, DoctorUserId = 1, DayOfWeek = 1 };
        var response = new ScheduleResponse(1, 1, "Dr. House", 1, new TimeOnly(8, 0), new TimeOnly(18, 0), 30, true);

        _mapper.Map<Schedule>(request).Returns(schedule);
        _mapper.Map<ScheduleResponse>(schedule).Returns(response);

        // Act
        var result = await _sut.CreateAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result.DoctorUserId.Should().Be(1);
        result.SlotDuration.Should().Be(30);
        await _uow.Schedules.Received(1).AddAsync(schedule);
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Delete_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        _uow.Schedules.GetByIdAsync(999).Returns((Schedule?)null);

        // Act
        var act = () => _sut.DeleteAsync(999, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Regra de agenda*");
    }

    [Fact]
    public async Task Delete_WithWrongClinic_ShouldThrowBusinessRuleException()
    {
        // Arrange
        _uow.Schedules.GetByIdAsync(1).Returns(new Schedule { Id = 1, ClinicId = 2 });

        // Act
        var act = () => _sut.DeleteAsync(1, 1);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Regra não pertence a esta clínica*");
    }

    [Fact]
    public async Task CreateException_WithoutReason_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var request = new CreateScheduleExceptionRequest(1, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), null, null, "");

        // Act
        var act = () => _sut.CreateExceptionAsync(1, request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Motivo da exceção é obrigatório*");
    }

    [Fact]
    public async Task CreateException_WithValidData_ShouldCreate()
    {
        // Arrange
        var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var request = new CreateScheduleExceptionRequest(1, tomorrow, null, null, "Feriado", false);
        var exception = new ScheduleException { Id = 1, Reason = "Feriado" };
        var response = new ScheduleExceptionResponse(1, 1, tomorrow, "Feriado", false);

        _uow.ScheduleExceptions.AddAsync(Arg.Any<ScheduleException>()).Returns(exception);
        _mapper.Map<ScheduleExceptionResponse>(Arg.Any<ScheduleException>()).Returns(response);

        // Act
        var result = await _sut.CreateExceptionAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result.Reason.Should().Be("Feriado");
        await _uow.ScheduleExceptions.Received(1).AddAsync(Arg.Any<ScheduleException>());
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteException_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        _uow.ScheduleExceptions.GetByIdAsync(999).Returns((ScheduleException?)null);

        // Act
        var act = () => _sut.DeleteExceptionAsync(999, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Exceção de agenda*");
    }
}