using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Appointments;
using VitalGest.Application.DTOs.Common;
using VitalGest.Application.DTOs.Exams;
using VitalGest.Application.DTOs.Patients;
using VitalGest.Application.DTOs.Prescriptions;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Entities;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

/// <summary>
/// Serviço de gestão de pacientes.
/// </summary>
public class PatientService : IPatientService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<PatientService> _logger;
    private readonly ICacheService _cache;

    private const string PatientCachePrefix = "patient:";

    public PatientService(
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<PatientService> logger,
        ICacheService cache)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<PagedResponse<PatientListResponse>> GetAllAsync(
        int clinicId,
        PagedRequest request,
        CancellationToken ct = default)
    {
        var cacheKey = $"{PatientCachePrefix}list:{clinicId}:p{request.Page}:s{request.PageSize}";

        // Tenta obter do cache
        var cached = await _cache.GetAsync<PagedResponse<PatientListResponse>>(cacheKey, ct);
        if (cached != null)
            return cached;

        var patients = await _uow.Patients.GetPagedAsync(
            request.Page,
            request.PageSize,
            p => p.ClinicId == clinicId && p.IsActive,
            ct);

        var count = await _uow.Patients.CountAsync(p => p.ClinicId == clinicId && p.IsActive, ct);

        var items = _mapper.Map<IEnumerable<PatientListResponse>>(patients);
        var result = PagedResponse.Create(items, request.Page, request.PageSize, count);

        // Armazena em cache por 2 minutos
        await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(2), ct);

        return result;
    }

    /// <inheritdoc />
    public async Task<PatientResponse> GetByIdAsync(int id, int clinicId, CancellationToken ct = default)
    {
        var cacheKey = $"{PatientCachePrefix}{id}:{clinicId}";

        var cached = await _cache.GetAsync<PatientResponse>(cacheKey, ct);
        if (cached != null)
            return cached;

        var patient = await _uow.Patients.GetByIdWithDetailsAsync(id, clinicId, ct)
            ?? throw new NotFoundException("Paciente", id);

        var result = _mapper.Map<PatientResponse>(patient);

        await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5), ct);

        return result;
    }

    /// <inheritdoc />
    public async Task<PatientResponse> CreateAsync(
        int clinicId,
        CreatePatientRequest request,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Criando paciente: {Name} na clínica {ClinicId}", request.Name, clinicId);

        // Verifica CPF duplicado
        if (!string.IsNullOrEmpty(request.CPF))
        {
            var existingPatient = await _uow.Patients.GetByCpfAsync(request.CPF, ct);
            if (existingPatient != null)
                throw new BusinessRuleException("CPF já cadastrado no sistema.", "CPF_ALREADY_EXISTS");
        }

        var patient = _mapper.Map<Patient>(request);
        patient.ClinicId = clinicId;
        patient.CreatedAt = DateTime.UtcNow;

        // Cria endereço se informado
        if (request.Address != null)
        {
            patient.Address = _mapper.Map<Address>(request.Address);
        }

        await _uow.Patients.AddAsync(patient, ct);
        await _uow.SaveChangesAsync(ct);

        // Invalida cache de listagem
        await _cache.RemoveByPrefixAsync($"{PatientCachePrefix}list:{clinicId}", ct);

        _logger.LogInformation("Paciente criado: {PatientId}", patient.Id);

        return _mapper.Map<PatientResponse>(patient);
    }

    /// <inheritdoc />
    public async Task<PatientResponse> UpdateAsync(
        int id,
        int clinicId,
        UpdatePatientRequest request,
        CancellationToken ct = default)
    {
        var patient = await _uow.Patients.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Paciente", id);

        // Verifica se pertence à clínica
        if (patient.ClinicId != clinicId)
            throw new BusinessRuleException("Paciente não pertence a esta clínica.", "WRONG_CLINIC");

        patient.Name = request.Name;
        patient.Phone = request.Phone;
        patient.RG = request.RG;
        patient.BirthDate = request.BirthDate;
        patient.Gender = request.Gender;
        patient.SecondaryPhone = request.SecondaryPhone;
        patient.Email = request.Email;
        patient.BloodType = request.BloodType;
        patient.Allergies = request.Allergies;
        patient.MedicalNotes = request.MedicalNotes;
        patient.EmergencyContact = request.EmergencyContact;
        patient.EmergencyPhone = request.EmergencyPhone;
        patient.InsurancePlanId = request.InsurancePlanId;
        patient.InsuranceCardNumber = request.InsuranceCardNumber;
        patient.InsuranceExpiryDate = request.InsuranceExpiryDate;
        patient.UpdatedAt = DateTime.UtcNow;

        await _uow.Patients.UpdateAsync(patient, ct);
        await _uow.SaveChangesAsync(ct);

        // Invalida caches
        await _cache.RemoveAsync($"{PatientCachePrefix}{id}:{clinicId}", ct);
        await _cache.RemoveByPrefixAsync($"{PatientCachePrefix}list:{clinicId}", ct);

        return _mapper.Map<PatientResponse>(patient);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, int clinicId, CancellationToken ct = default)
    {
        var patient = await _uow.Patients.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Paciente", id);

        if (patient.ClinicId != clinicId)
            throw new BusinessRuleException("Paciente não pertence a esta clínica.", "WRONG_CLINIC");

        await _uow.Patients.DeleteAsync(patient, ct); // Soft delete
        await _uow.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"{PatientCachePrefix}{id}:{clinicId}", ct);
        await _cache.RemoveByPrefixAsync($"{PatientCachePrefix}list:{clinicId}", ct);

        _logger.LogInformation("Paciente desativado: {PatientId}", id);
    }

    /// <inheritdoc />
    public async Task<PatientHistoryResponse> GetHistoryAsync(
        int id,
        int clinicId,
        CancellationToken ct = default)
    {
        var patient = await _uow.Patients.GetByIdWithDetailsAsync(id, clinicId, ct)
            ?? throw new NotFoundException("Paciente", id);

        var recentAppointments = await _uow.Appointments.GetByPatientIdAsync(id, clinicId, ct);
        var recentExams = await _uow.Exams.GetByPatientIdAsync(id, clinicId, ct);
        var recentPrescriptions = await _uow.Prescriptions.GetByPatientIdAsync(id, clinicId, ct);

        return new PatientHistoryResponse(
            _mapper.Map<PatientResponse>(patient),
            _mapper.Map<IEnumerable<AppointmentSimpleResponse>>(recentAppointments.Take(5)),
            _mapper.Map<IEnumerable<ExamSimpleResponse>>(recentExams.Take(5)),
            _mapper.Map<IEnumerable<PrescriptionSimpleResponse>>(recentPrescriptions.Take(5))
        );
    }

    /// <inheritdoc />
    public async Task<PagedResponse<PatientListResponse>> SearchAsync(
        int clinicId,
        string query,
        PagedRequest request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return PagedResponse.Create(Enumerable.Empty<PatientListResponse>(), 1, request.PageSize, 0);

        var patients = await _uow.Patients.SearchAsync(query, clinicId, request.Page, request.PageSize, ct);
        var items = _mapper.Map<IEnumerable<PatientListResponse>>(patients);

        var count = patients.Count(); // Simplificado - idealmente teria uma busca de count separada

        return PagedResponse.Create(items, request.Page, request.PageSize, count);
    }
}