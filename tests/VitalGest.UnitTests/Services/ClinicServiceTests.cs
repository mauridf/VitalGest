using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Clinics;
using VitalGest.Application.Services;
using VitalGest.Core.Entities;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.UnitTests.Services;

/// <summary>
/// Testes unitários para o ClinicService.
/// </summary>
public class ClinicServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<ClinicService> _logger;
    private readonly ClinicService _sut;

    public ClinicServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<ClinicService>>();
        _sut = new ClinicService(_uow, _mapper, _logger);
    }

    [Fact]
    public async Task Create_WithDuplicateCNPJ_ShouldThrowBusinessRuleException()
    {
        var request = new CreateClinicRequest("Clínica Teste", "Teste Ltda", "12.345.678/0001-90", "(11) 99999-9999", "teste@teste.com");
        _uow.Clinics.GetByCnpjAsync("12.345.678/0001-90").Returns(new Clinic { Id = 1, Name = "Existente" });

        var act = () => _sut.CreateAsync(request);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*CNPJ já cadastrado*");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldThrowNotFoundException()
    {
        _uow.Clinics.GetByIdWithDetailsAsync(999).Returns((Clinic?)null);

        var act = () => _sut.GetByIdAsync(999);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Clínica*");
    }

    [Fact]
    public async Task Update_WithInvalidId_ShouldThrowNotFoundException()
    {
        var request = new UpdateClinicRequest("Updated", "Updated Ltda", "(11) 11111-1111", "upd@teste.com");
        _uow.Clinics.GetByIdAsync(999).Returns((Clinic?)null);

        var act = () => _sut.UpdateAsync(999, request);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Clínica*");
    }

    [Fact]
    public async Task GetStats_ShouldReturnStatsResponse()
    {
        _uow.Patients.CountAsync(Arg.Any<System.Linq.Expressions.Expression<System.Func<Patient, bool>>>()).Returns(50);
        _uow.Appointments.CountTodayAsync(1).Returns(10);

        var result = await _sut.GetStatsAsync(1);

        result.Should().NotBeNull();
        result.TotalPatients.Should().Be(50);
        result.TodayAppointments.Should().Be(10);
    }

    [Fact]
    public async Task CreateDepartment_ShouldMapAndAdd()
    {
        var request = new CreateDepartmentRequest("Cardiologia");
        var clinicId = 1;
        var department = new Department { Id = 1, Name = "Cardiologia", ClinicId = clinicId };
        var response = new DepartmentResponse(1, "Cardiologia", null, true, DateTime.UtcNow);

        _mapper.Map<Department>(request).Returns(department);
        _mapper.Map<DepartmentResponse>(department).Returns(response);

        var result = await _sut.CreateDepartmentAsync(clinicId, request);

        result.Should().NotBeNull();
        result.Name.Should().Be("Cardiologia");
        await _uow.Departments.Received(1).AddAsync(department);
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GetDepartments_ShouldReturnActiveDepartments()
    {
        var departments = new List<Department>
        {
            new() { Id = 1, Name = "Cardio", ClinicId = 1, IsActive = true }
        };
        var responses = new[] { new DepartmentResponse(1, "Cardio", null, true, DateTime.UtcNow) };

        _uow.Departments.FindAsync(Arg.Any<System.Linq.Expressions.Expression<System.Func<Department, bool>>>()).Returns(departments);
        _mapper.Map<IEnumerable<DepartmentResponse>>(departments).Returns(responses);

        var result = await _sut.GetDepartmentsAsync(1);

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Cardio");
    }
}
