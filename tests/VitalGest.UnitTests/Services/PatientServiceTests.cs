using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Appointments;
using VitalGest.Application.DTOs.Common;
using VitalGest.Application.DTOs.Exams;
using VitalGest.Application.DTOs.Patients;
using VitalGest.Application.DTOs.Prescriptions;
using VitalGest.Application.Interfaces;
using VitalGest.Application.Services;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.UnitTests.Services;

/// <summary>
/// Testes unitários para o PatientService.
/// </summary>
public class PatientServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<PatientService> _logger;
    private readonly ICacheService _cache;
    private readonly PatientService _sut;

    public PatientServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<PatientService>>();
        _cache = Substitute.For<ICacheService>();
        _sut = new PatientService(_uow, _mapper, _logger, _cache);
    }

    [Fact]
    public async Task Create_WithDuplicateCPF_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var request = new CreatePatientRequest(
            "John Doe",
            "(11) 99999-9999",
            CPF: "123.456.789-00"
        );

        _uow.Patients.GetByCpfAsync("123.456.789-00")
            .Returns(new Patient { Id = 999, Name = "Existing Patient", CPF = "123.456.789-00" });

        // Act
        var act = () => _sut.CreateAsync(1, request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*CPF já cadastrado*");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        _uow.Patients.GetByIdWithDetailsAsync(999, 1)
            .Returns((Patient?)null);

        // Act
        var act = () => _sut.GetByIdAsync(999, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task Delete_ShouldPerformSoftDelete()
    {
        // Arrange
        var patient = new Patient
        {
            Id = 1,
            ClinicId = 1,
            IsActive = true
        };

        _uow.Patients.GetByIdAsync(1).Returns(patient);

        // Act
        await _sut.DeleteAsync(1, 1);

        // Assert
        await _uow.Patients.Received(1).DeleteAsync(patient);
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Create_WithValidData_ShouldCreatePatient()
    {
        // Arrange
        var request = new CreatePatientRequest("Jane Doe", "(11) 98888-8888");
        var patient = new Patient { Id = 1, Name = "Jane Doe", ClinicId = 1 };
        var response = new PatientResponse(1, "Jane Doe", null, null, null, null, "(11) 98888-8888", null, null, null, null, null, null, null, null, null, true, DateTime.UtcNow);

        _mapper.Map<Patient>(Arg.Any<CreatePatientRequest>()).Returns(patient);
        _uow.Patients.AddAsync(Arg.Any<Patient>()).Returns(patient);
        _mapper.Map<PatientResponse>(Arg.Any<Patient>()).Returns(response);

        // Act
        var result = await _sut.CreateAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Jane Doe");
        await _uow.Patients.Received(1).AddAsync(Arg.Any<Patient>());
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Search_WithShortQuery_ShouldReturnEmpty()
    {
        // Arrange
        var request = new PagedRequest();

        // Act
        var result = await _sut.SearchAsync(1, "A", request);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetHistory_ShouldReturnPatientHistory()
    {
        // Arrange
        var patient = new Patient { Id = 1, ClinicId = 1, Name = "John Doe" };
        _uow.Patients.GetByIdWithDetailsAsync(1, 1).Returns(patient);
        _uow.Appointments.GetByPatientIdAsync(1, 1).Returns([]);
        _uow.Exams.GetByPatientIdAsync(1, 1).Returns([]);
        _uow.Prescriptions.GetByPatientIdAsync(1, 1).Returns([]);
        _mapper.Map<PatientResponse>(patient).Returns(new PatientResponse(1, "John Doe", null, null, null, null, null, null, null, null, null, null, null, null, null, null, true, DateTime.UtcNow));
        _mapper.Map<IEnumerable<AppointmentSimpleResponse>>(Arg.Any<IEnumerable<Appointment>>()).Returns([]);
        _mapper.Map<IEnumerable<ExamSimpleResponse>>(Arg.Any<IEnumerable<Exam>>()).Returns([]);
        _mapper.Map<IEnumerable<PrescriptionSimpleResponse>>(Arg.Any<IEnumerable<Prescription>>()).Returns([]);

        // Act
        var result = await _sut.GetHistoryAsync(1, 1);

        // Assert
        result.Should().NotBeNull();
        result.Patient.Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task Update_WithWrongClinic_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var patient = new Patient { Id = 1, ClinicId = 2 };
        var request = new UpdatePatientRequest("Updated", "(11) 11111-1111");

        _uow.Patients.GetByIdAsync(1).Returns(patient);

        // Act
        var act = () => _sut.UpdateAsync(1, 1, request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Paciente não pertence a esta clínica*");
    }
}