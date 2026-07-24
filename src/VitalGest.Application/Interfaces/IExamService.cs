using VitalGest.Application.DTOs.Exams;
using VitalGest.Application.DTOs.Common;

namespace VitalGest.Application.Interfaces;

public interface IExamService
{
    Task<PagedResponse<ExamResponse>> GetAllAsync(int clinicId, PagedRequest request, CancellationToken ct = default);
    Task<ExamResponse> GetByIdAsync(int id, int clinicId, CancellationToken ct = default);
    Task<ExamResponse> CreateAsync(int clinicId, int doctorUserId, CreateExamRequest request, CancellationToken ct = default);
    Task<ExamResponse> UpdateStatusAsync(int id, int clinicId, UpdateExamStatusRequest request, CancellationToken ct = default);
    Task<ExamResultResponse> AddResultAsync(int examId, int clinicId, int performedById, CreateExamResultRequest request, CancellationToken ct = default);
    Task<IEnumerable<ExamTypeResponse>> GetExamTypesAsync(CancellationToken ct = default);
}