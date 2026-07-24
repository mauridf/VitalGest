using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Prescriptions;
using VitalGest.Application.Services;
using VitalGest.Core.Entities;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.UnitTests.Services;

/// <summary>
/// Testes unitários para o PrescriptionService.
/// </summary>
public class PrescriptionServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<PrescriptionService> _logger;
    private readonly PrescriptionService _sut;

    public PrescriptionServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<PrescriptionService>>();
        _sut = new PrescriptionService(_uow, _mapper, _logger);
    }

    [Fact]
    public async Task Create_WithNoItems_ShouldThrowBusinessRuleException()
    {
        var request = new CreatePrescriptionRequest(1, []);

        var act = () => _sut.CreateAsync(1, 1, request);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Prescrição deve ter no mínimo 1 item*");
    }

    [Fact]
    public async Task Delete_ByDifferentDoctor_ShouldThrowBusinessRuleException()
    {
        var prescription = new Prescription { Id = 1, DoctorUserId = 1 };
        _uow.Prescriptions.GetByIdAsync(1).Returns(prescription);

        var act = () => _sut.DeleteAsync(1, 1, 2);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Apenas o médico que criou a prescrição pode excluí-la*");
    }

    [Fact]
    public async Task Delete_ByOwnerDoctor_ShouldSucceed()
    {
        var prescription = new Prescription { Id = 1, DoctorUserId = 1 };
        _uow.Prescriptions.GetByIdAsync(1).Returns(prescription);

        await _sut.DeleteAsync(1, 1, 1);

        await _uow.Prescriptions.Received(1).DeleteAsync(prescription);
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldThrowNotFoundException()
    {
        _uow.Prescriptions.GetByIdWithItemsAsync(999, 1).Returns((Prescription?)null);

        var act = () => _sut.GetByIdAsync(999, 1);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Prescrição*");
    }
}
