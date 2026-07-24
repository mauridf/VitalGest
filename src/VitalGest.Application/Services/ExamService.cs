using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Common;
using VitalGest.Application.DTOs.Exams;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

public class ExamService : IExamService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<ExamService> _logger;

    public ExamService(IUnitOfWork uow, IMapper mapper, ILogger<ExamService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResponse<ExamResponse>> GetAllAsync(int clinicId, PagedRequest request, CancellationToken ct = default)
    {
        var exams = await _uow.Exams.GetPagedAsync(request.Page, request.PageSize, e => e.ClinicId == clinicId, ct);
        var count = await _uow.Exams.CountAsync(e => e.ClinicId == clinicId, ct);
        return PagedResponse.Create(_mapper.Map<IEnumerable<ExamResponse>>(exams), request.Page, request.PageSize, count);
    }

    public async Task<ExamResponse> GetByIdAsync(int id, int clinicId, CancellationToken ct = default)
    {
        var exam = await _uow.Exams.GetByIdWithResultAsync(id, clinicId, ct)
            ?? throw new NotFoundException("Exame", id);
        return _mapper.Map<ExamResponse>(exam);
    }

    public async Task<ExamResponse> CreateAsync(int clinicId, int doctorUserId, CreateExamRequest request, CancellationToken ct = default)
    {
        var exam = _mapper.Map<Exam>(request);
        exam.ClinicId = clinicId;
        exam.DoctorUserId = doctorUserId;
        exam.Status = ExamStatus.Requested;
        exam.RequestDate = DateTime.UtcNow;
        exam.CreatedAt = DateTime.UtcNow;

        await _uow.Exams.AddAsync(exam, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Exame solicitado: {ExamId}, Tipo: {ExamTypeId}", exam.Id, request.ExamTypeId);

        return _mapper.Map<ExamResponse>(exam);
    }

    public async Task<ExamResponse> UpdateStatusAsync(int id, int clinicId, UpdateExamStatusRequest request, CancellationToken ct = default)
    {
        var exam = await _uow.Exams.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Exame", id);

        if (exam.ClinicId != clinicId)
            throw new BusinessRuleException("Exame não pertence a esta clínica.", "WRONG_CLINIC");

        exam.Status = request.Status;
        exam.UpdatedAt = DateTime.UtcNow;

        await _uow.Exams.UpdateAsync(exam, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<ExamResponse>(exam);
    }

    public async Task<ExamResultResponse> AddResultAsync(int examId, int clinicId, int performedById, CreateExamResultRequest request, CancellationToken ct = default)
    {
        var exam = await _uow.Exams.GetByIdAsync(examId, ct)
            ?? throw new NotFoundException("Exame", examId);

        if (exam.Status != ExamStatus.InAnalysis && exam.Status != ExamStatus.Collected)
            throw new BusinessRuleException("Exame deve estar em análise para receber resultado.", "INVALID_EXAM_STATUS");

        var result = new ExamResult
        {
            ExamId = examId,
            ResultDate = DateTime.UtcNow,
            Summary = request.Summary,
            ResultJson = request.ResultJson,
            FileUrl = request.FileUrl,
            PerformedById = performedById,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Exams.AddResultAsync(result, ct);

        // Atualiza status do exame para Pronto
        exam.Status = ExamStatus.Ready;
        exam.UpdatedAt = DateTime.UtcNow;
        await _uow.Exams.UpdateAsync(exam, ct);

        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Resultado de exame registrado: ExamId={ExamId}", examId);

        return _mapper.Map<ExamResultResponse>(result);
    }

    public async Task<IEnumerable<ExamTypeResponse>> GetExamTypesAsync(CancellationToken ct = default)
    {
        var types = await _uow.ExamTypes.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<ExamTypeResponse>>(types);
    }
}