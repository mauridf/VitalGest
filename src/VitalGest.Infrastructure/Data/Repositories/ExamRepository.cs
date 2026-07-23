using Microsoft.EntityFrameworkCore;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;
using VitalGest.Core.Interfaces;
using VitalGest.Infrastructure.Data.Context;

namespace VitalGest.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório especializado para Exames.
/// </summary>
public class ExamRepository : Repository<Exam>, IExamRepository
{
    public ExamRepository(VitalGestDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<IEnumerable<Exam>> GetByPatientIdAsync(
        int patientId,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.ExamType)
            .Include(e => e.Doctor)
            .Include(e => e.Result)
            .Where(e => e.PatientId == patientId && e.ClinicId == clinicId)
            .OrderByDescending(e => e.RequestDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Exam>> GetByStatusAsync(
        ExamStatus status,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.ExamType)
            .Include(e => e.Patient)
            .Where(e => e.ClinicId == clinicId && e.Status == status)
            .OrderByDescending(e => e.RequestDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Exam?> GetByIdWithResultAsync(
        int examId,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.ExamType)
            .Include(e => e.Patient)
            .Include(e => e.Doctor)
            .Include(e => e.Result)
                .ThenInclude(r => r!.PerformedBy)
            .Include(e => e.Result)
                .ThenInclude(r => r!.ReviewedBy)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == examId && e.ClinicId == clinicId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ExamResult> AddResultAsync(
        ExamResult result,
        CancellationToken cancellationToken = default)
    {
        await _context.ExamResults.AddAsync(result, cancellationToken);
        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Exam>> GetPendingResultsAsync(
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.ExamType)
            .Include(e => e.Patient)
            .Where(e => e.ClinicId == clinicId
                && (e.Status == ExamStatus.Requested
                    || e.Status == ExamStatus.Collected
                    || e.Status == ExamStatus.InAnalysis))
            .OrderBy(e => e.RequestDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}