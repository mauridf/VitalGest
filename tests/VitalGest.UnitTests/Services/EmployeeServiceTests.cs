using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Employees;
using VitalGest.Application.Services;
using VitalGest.Core.Entities;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.UnitTests.Services;

/// <summary>
/// Testes unitários para o EmployeeService.
/// </summary>
public class EmployeeServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<EmployeeService> _logger;
    private readonly EmployeeService _sut;

    public EmployeeServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<EmployeeService>>();
        _sut = new EmployeeService(_uow, _mapper, _logger);
    }

    [Fact]
    public async Task Create_WithExistingEmail_ShouldThrowBusinessRuleException()
    {
        var request = new CreateEmployeeRequest("John", "john@email.com", "(11) 99999-9999", null, null);
        _uow.Users.GetByEmailAsync("john@email.com").Returns(new User { Id = 1, Email = "john@email.com" });

        var act = () => _sut.CreateAsync(1, request);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*E-mail já cadastrado*");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldThrowNotFoundException()
    {
        _uow.Users.GetByIdAsync(999).Returns((User?)null);

        var act = () => _sut.GetByIdAsync(999, 1);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Funcionário*");
    }

    [Fact]
    public async Task Update_WithInvalidId_ShouldThrowNotFoundException()
    {
        _uow.Users.GetByIdAsync(999).Returns((User?)null);
        var request = new UpdateEmployeeRequest("Updated", "upd@email.com", null, null, null);

        var act = () => _sut.UpdateAsync(999, 1, request);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Funcionário*");
    }

    [Fact]
    public async Task Delete_ShouldPerformSoftDelete()
    {
        var user = new User { Id = 1, IsActive = true };
        _uow.Users.GetByIdAsync(1).Returns(user);

        await _sut.DeleteAsync(1, 1);

        user.IsActive.Should().BeFalse();
        await _uow.Users.Received(1).UpdateAsync(user);
        await _uow.Received(1).SaveChangesAsync();
    }
}
