using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Common;
using VitalGest.Application.DTOs.Employees;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(IUnitOfWork uow, IMapper mapper, ILogger<EmployeeService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResponse<EmployeeResponse>> GetAllAsync(int clinicId, PagedRequest request, CancellationToken ct = default)
    {
        var employees = await _uow.Users.GetPagedAsync(request.Page, request.PageSize, u => u.ClinicUsers.Any(cu => cu.ClinicId == clinicId), ct);
        var count = await _uow.Users.CountAsync(u => u.ClinicUsers.Any(cu => cu.ClinicId == clinicId), ct);
        return PagedResponse.Create(_mapper.Map<IEnumerable<EmployeeResponse>>(employees), request.Page, request.PageSize, count);
    }

    public async Task<EmployeeResponse> GetByIdAsync(int id, int clinicId, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Funcionário", id);
        return _mapper.Map<EmployeeResponse>(user);
    }

    public async Task<EmployeeResponse> CreateAsync(int clinicId, CreateEmployeeRequest request, CancellationToken ct = default)
    {
        if (await _uow.Users.GetByEmailAsync(request.Email, ct) != null)
            throw new BusinessRuleException("E-mail já cadastrado.", "EMAIL_ALREADY_EXISTS");

        var user = _mapper.Map<User>(request);
        user.CreatedAt = DateTime.UtcNow;

        await _uow.Users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<EmployeeResponse>(user);
    }

    public async Task<EmployeeResponse> UpdateAsync(int id, int clinicId, UpdateEmployeeRequest request, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Funcionário", id);

        user.Name = request.Name;
        user.Email = request.Email;
        user.Phone = request.Phone;
        user.CPF = request.CPF;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _uow.Users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<EmployeeResponse>(user);
    }

    public async Task DeleteAsync(int id, int clinicId, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Funcionário", id);

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _uow.Users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<EmployeeResponse>> GetDoctorsAsync(int clinicId, CancellationToken ct = default)
    {
        var doctors = await _uow.Users.FindAsync(u => u.ClinicUsers.Any(cu => cu.ClinicId == clinicId) && u.Role == UserRole.User, ct);
        return _mapper.Map<IEnumerable<EmployeeResponse>>(doctors);
    }

    public async Task<IEnumerable<PositionResponse>> GetPositionsAsync(CancellationToken ct = default)
    {
        var positions = await _uow.Positions.FindAsync(p => p.IsActive, ct);
        return _mapper.Map<IEnumerable<PositionResponse>>(positions);
    }
}
