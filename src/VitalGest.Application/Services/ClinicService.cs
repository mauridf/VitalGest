using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Clinics;
using VitalGest.Application.DTOs.Common;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Entities;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

/// <summary>
/// Serviço de gestão de clínicas (tenants).
/// </summary>
public class ClinicService : IClinicService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<ClinicService> _logger;

    public ClinicService(IUnitOfWork uow, IMapper mapper, ILogger<ClinicService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ClinicResponse> CreateAsync(CreateClinicRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Criando nova clínica: {Name}", request.Name);

        // Verifica CNPJ único
        if (await _uow.Clinics.GetByCnpjAsync(request.CNPJ, ct) != null)
            throw new BusinessRuleException("CNPJ já cadastrado.", "CNPJ_ALREADY_EXISTS");

        // Cria endereço se informado
        Address? address = null;
        if (request.Address != null)
        {
            address = _mapper.Map<Address>(request.Address);
        }

        var clinic = _mapper.Map<Clinic>(request);
        clinic.Address = address;
        clinic.CreatedAt = DateTime.UtcNow;

        await _uow.Clinics.AddAsync(clinic, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Clínica criada: {ClinicId}", clinic.Id);

        return _mapper.Map<ClinicResponse>(clinic);
    }

    /// <inheritdoc />
    public async Task<ClinicResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var clinic = await _uow.Clinics.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException("Clínica", id);

        return _mapper.Map<ClinicResponse>(clinic);
    }

    /// <inheritdoc />
    public async Task<ClinicResponse> UpdateAsync(int id, UpdateClinicRequest request, CancellationToken ct = default)
    {
        var clinic = await _uow.Clinics.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Clínica", id);

        clinic.Name = request.Name;
        clinic.CorporateName = request.CorporateName;
        clinic.Phone = request.Phone;
        clinic.Email = request.Email;
        clinic.Description = request.Description;
        clinic.SecondaryPhone = request.SecondaryPhone;
        clinic.Website = request.Website;
        clinic.OpeningHours = request.OpeningHours;
        clinic.UpdatedAt = DateTime.UtcNow;

        await _uow.Clinics.UpdateAsync(clinic, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<ClinicResponse>(clinic);
    }

    /// <inheritdoc />
    public async Task<ClinicStatsResponse> GetStatsAsync(int clinicId, CancellationToken ct = default)
    {
        var totalPatients = await _uow.Patients.CountAsync(p => p.ClinicId == clinicId && p.IsActive, ct);
        var todayAppointments = await _uow.Appointments.CountTodayAsync(clinicId, ct);

        // Conta médicos (usuários com position "Médico")
        var allClinicUsers = await _uow.Appointments.CountTodayAsync(clinicId, ct); // Reutilizar para estimativa

        return new ClinicStatsResponse(
            totalPatients,
            0, // totalAppointments - será implementado depois
            allClinicUsers, // estimativa
            todayAppointments,
            0 // monthlyRevenue - será implementado depois
        );
    }

    /// <inheritdoc />
    public async Task<DepartmentResponse> CreateDepartmentAsync(int clinicId, CreateDepartmentRequest request, CancellationToken ct = default)
    {
        var department = _mapper.Map<Department>(request);
        department.ClinicId = clinicId;
        department.CreatedAt = DateTime.UtcNow;

        await _uow.Departments.AddAsync(department, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<DepartmentResponse>(department);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DepartmentResponse>> GetDepartmentsAsync(int clinicId, CancellationToken ct = default)
    {
        var departments = await _uow.Departments.FindAsync(d => d.ClinicId == clinicId && d.IsActive, ct);
        return _mapper.Map<IEnumerable<DepartmentResponse>>(departments);
    }
}