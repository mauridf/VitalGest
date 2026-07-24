using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Insurance;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Entities;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

public class InsuranceService : IInsuranceService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<InsuranceService> _logger;

    public InsuranceService(IUnitOfWork uow, IMapper mapper, ILogger<InsuranceService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<InsurancePlanResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var plans = await _uow.InsurancePlans.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<InsurancePlanResponse>>(plans);
    }

    public async Task<InsurancePlanResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var plan = await _uow.InsurancePlans.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Plano de saúde", id);
        return _mapper.Map<InsurancePlanResponse>(plan);
    }

    public async Task<InsurancePlanResponse> CreateAsync(CreateInsurancePlanRequest request, CancellationToken ct = default)
    {
        var plan = _mapper.Map<InsurancePlan>(request);
        plan.CreatedAt = DateTime.UtcNow;

        await _uow.InsurancePlans.AddAsync(plan, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<InsurancePlanResponse>(plan);
    }

    public async Task<InsurancePlanResponse> UpdateAsync(int id, UpdateInsurancePlanRequest request, CancellationToken ct = default)
    {
        var plan = await _uow.InsurancePlans.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Plano de saúde", id);

        plan.Name = request.Name;
        plan.Phone = request.Phone;
        plan.Email = request.Email;
        plan.IsActive = request.IsActive;
        plan.UpdatedAt = DateTime.UtcNow;

        await _uow.InsurancePlans.UpdateAsync(plan, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<InsurancePlanResponse>(plan);
    }

    public async Task<IEnumerable<InsuranceCoverageResponse>> GetCoveragesAsync(int planId, CancellationToken ct = default)
    {
        var coverages = await _uow.InsuranceCoverages.FindAsync(c => c.InsurancePlanId == planId, ct);
        return _mapper.Map<IEnumerable<InsuranceCoverageResponse>>(coverages);
    }

    public async Task<InsuranceCoverageResponse> AddCoverageAsync(int planId, CreateInsuranceCoverageRequest request, CancellationToken ct = default)
    {
        var coverage = _mapper.Map<InsuranceCoverage>(request);
        coverage.InsurancePlanId = planId;
        coverage.CreatedAt = DateTime.UtcNow;

        await _uow.InsuranceCoverages.AddAsync(coverage, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<InsuranceCoverageResponse>(coverage);
    }
}