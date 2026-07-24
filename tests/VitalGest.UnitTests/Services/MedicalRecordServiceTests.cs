using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.MedicalRecords;
using VitalGest.Application.Services;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.UnitTests.Services;

/// <summary>
/// Testes unitários para o MedicalRecordService.
/// </summary>
public class MedicalRecordServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<MedicalRecordService> _logger;
    private readonly MedicalRecordService _sut;

    public MedicalRecordServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<MedicalRecordService>>();
        _sut = new MedicalRecordService(_uow, _mapper, _logger);
    }

    [Fact]
    public async Task GetByPatient_WhenRecordDoesNotExist_ShouldAutoCreate()
    {
        _uow.MedicalRecords.GetByPatientIdWithEntriesAsync(1, 1).Returns((MedicalRecord?)null);
        var createdRecord = new MedicalRecord { Id = 1, PatientId = 1, ClinicId = 1 };
        _uow.MedicalRecords.GetOrCreateAsync(1, 1).Returns(createdRecord);
        _mapper.Map<MedicalRecordResponse>(createdRecord).Returns(new MedicalRecordResponse(1, 1, "John", [], DateTime.UtcNow));

        var result = await _sut.GetByPatientAsync(1, 1);

        result.Should().NotBeNull();
        result.PatientId.Should().Be(1);
        await _uow.MedicalRecords.Received(1).GetOrCreateAsync(1, 1);
    }

    [Fact]
    public async Task GetByPatient_WhenRecordExists_ShouldReturnExisting()
    {
        var record = new MedicalRecord { Id = 1, PatientId = 1, ClinicId = 1, Entries = new List<MedicalRecordEntry>() };
        _uow.MedicalRecords.GetByPatientIdWithEntriesAsync(1, 1).Returns(record);
        _mapper.Map<MedicalRecordResponse>(record).Returns(new MedicalRecordResponse(1, 1, "John", [], DateTime.UtcNow));

        var result = await _sut.GetByPatientAsync(1, 1);

        result.Should().NotBeNull();
        await _uow.MedicalRecords.DidNotReceive().GetOrCreateAsync(Arg.Any<int>(), Arg.Any<int>());
    }

    [Fact]
    public async Task AddEntry_ShouldCreateAndSave()
    {
        var request = new CreateMedicalRecordEntryRequest(1, MedicalRecordEntryType.Evolution, "Paciente apresenta melhora.");
        var record = new MedicalRecord { Id = 1, PatientId = 1, ClinicId = 1 };
        var entry = new MedicalRecordEntry { Id = 1, Description = "Paciente apresenta melhora." };
        var response = new MedicalRecordEntryResponse(1, 1, "Dr. House", "Consultation", "Paciente apresenta melhora.", false, DateTime.UtcNow);

        _uow.MedicalRecords.GetOrCreateAsync(1, 1).Returns(record);
        _uow.MedicalRecords.AddEntryAsync(Arg.Any<MedicalRecordEntry>()).Returns(entry);
        _mapper.Map<MedicalRecordEntryResponse>(Arg.Any<MedicalRecordEntry>()).Returns(response);

        var result = await _sut.AddEntryAsync(1, 1, request);

        result.Should().NotBeNull();
        result.Description.Should().Be("Paciente apresenta melhora.");
        await _uow.MedicalRecords.Received(1).AddEntryAsync(Arg.Any<MedicalRecordEntry>());
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GetEntry_WithInvalidId_ShouldThrowNotFoundException()
    {
        _uow.MedicalRecordEntries.GetByIdAsync(999).Returns((MedicalRecordEntry?)null);

        var act = () => _sut.GetEntryAsync(999);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Entrada de prontuário*");
    }

    [Fact]
    public async Task GetSummary_WhenNoRecord_ShouldReturnDefault()
    {
        _uow.MedicalRecords.GetByPatientIdWithEntriesAsync(1, 1).Returns((MedicalRecord?)null);

        var result = await _sut.GetSummaryAsync(1, 1);

        result.Should().NotBeNull();
        result.PatientName.Should().Be("N/A");
        result.TotalEntries.Should().Be(0);
    }
}
