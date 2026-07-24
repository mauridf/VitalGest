using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Atests;
using VitalGest.Application.Services;
using VitalGest.Core.Entities;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.UnitTests.Services;

/// <summary>
/// Testes unitários para o AtestService.
/// </summary>
public class AtestServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<AtestService> _logger;
    private readonly AtestService _sut;

    public AtestServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<AtestService>>();
        _sut = new AtestService(_uow, _mapper, _logger);
    }

    [Fact]
    public async Task Create_WithEndDateBeforeStartDate_ShouldThrowBusinessRuleException()
    {
        var request = new CreateAtestRequest(1, new DateOnly(2026, 7, 20), new DateOnly(2026, 7, 18), "Repouso");

        var act = () => _sut.CreateAsync(1, 1, request);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Data final deve ser posterior à data inicial*");
    }

    [Fact]
    public async Task Create_WithSameStartAndEndDate_ShouldSucceed()
    {
        var request = new CreateAtestRequest(1, new DateOnly(2026, 7, 20), new DateOnly(2026, 7, 20), "Repouso de 1 dia");
        var atest = new Atest { Id = 1, RestDays = 1 };
        var response = new AtestResponse(1, 1, "John", 1, "Dr. House", DateTime.UtcNow, new DateOnly(2026, 7, 20), new DateOnly(2026, 7, 20), null, "Repouso de 1 dia", 1);

        _uow.Atests.AddAsync(Arg.Any<Atest>()).Returns(atest);
        _mapper.Map<AtestResponse>(Arg.Any<Atest>()).Returns(response);

        var result = await _sut.CreateAsync(1, 1, request);

        result.Should().NotBeNull();
        result.RestDays.Should().Be(1);
        await _uow.Atests.Received(1).AddAsync(Arg.Any<Atest>());
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Delete_ByDifferentDoctor_ShouldThrowBusinessRuleException()
    {
        var atest = new Atest { Id = 1, DoctorUserId = 1 };
        _uow.Atests.GetByIdAsync(1).Returns(atest);

        var act = () => _sut.DeleteAsync(1, 1, 2);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Apenas o médico que emitiu o atestado pode excluí-lo*");
    }

    [Fact]
    public async Task Delete_ByOwnerDoctor_ShouldSucceed()
    {
        var atest = new Atest { Id = 1, DoctorUserId = 1 };
        _uow.Atests.GetByIdAsync(1).Returns(atest);

        await _sut.DeleteAsync(1, 1, 1);

        await _uow.Atests.Received(1).DeleteAsync(atest);
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldThrowNotFoundException()
    {
        _uow.Atests.GetByIdAsync(999).Returns((Atest?)null);

        var act = () => _sut.GetByIdAsync(999, 1);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Atestado*");
    }
}
