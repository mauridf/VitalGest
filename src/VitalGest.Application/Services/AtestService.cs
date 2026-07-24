using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Atests;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Entities;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

public class AtestService : IAtestService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<AtestService> _logger;

    public AtestService(IUnitOfWork uow, IMapper mapper, ILogger<AtestService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<AtestResponse>> GetByPatientAsync(int patientId, int clinicId, CancellationToken ct = default)
    {
        var atests = await _uow.Atests.FindAsync(a => a.PatientId == patientId && a.ClinicId == clinicId, ct);
        return _mapper.Map<IEnumerable<AtestResponse>>(atests);
    }

    public async Task<AtestResponse> GetByIdAsync(int id, int clinicId, CancellationToken ct = default)
    {
        var atest = await _uow.Atests.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Atestado", id);
        return _mapper.Map<AtestResponse>(atest);
    }

    public async Task<AtestResponse> CreateAsync(int clinicId, int doctorUserId, CreateAtestRequest request, CancellationToken ct = default)
    {
        if (request.EndDate < request.StartDate)
            throw new BusinessRuleException("Data final deve ser posterior à data inicial.", "INVALID_DATES");

        var restDays = (request.EndDate.ToDateTime(TimeOnly.MinValue) - request.StartDate.ToDateTime(TimeOnly.MinValue)).Days + 1;

        if (restDays < 1)
            throw new BusinessRuleException("Atestado deve ter período mínimo de 1 dia.", "INVALID_PERIOD");

        var atest = new Atest
        {
            ClinicId = clinicId,
            PatientId = request.PatientId,
            DoctorUserId = doctorUserId,
            AppointmentId = request.AppointmentId,
            IssueDate = DateTime.UtcNow,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            CID = request.CID,
            Description = request.Description,
            RestDays = restDays,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Atests.AddAsync(atest, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Atestado emitido: {AtestId}, Dias: {RestDays}", atest.Id, restDays);

        return _mapper.Map<AtestResponse>(atest);
    }

    public async Task DeleteAsync(int id, int clinicId, int doctorUserId, CancellationToken ct = default)
    {
        var atest = await _uow.Atests.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Atestado", id);

        if (atest.DoctorUserId != doctorUserId)
            throw new BusinessRuleException("Apenas o médico que emitiu o atestado pode excluí-lo.", "NOT_OWNER");

        await _uow.Atests.DeleteAsync(atest, ct);
        await _uow.SaveChangesAsync(ct);
    }
}