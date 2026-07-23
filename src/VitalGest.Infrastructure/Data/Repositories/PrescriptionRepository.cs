using Microsoft.EntityFrameworkCore;
using VitalGest.Core.Entities;
using VitalGest.Core.Interfaces;
using VitalGest.Infrastructure.Data.Context;

namespace VitalGest.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório especializado para Prescrições.
/// </summary>
public class PrescriptionRepository : Repository<Prescription>, IPrescriptionRepository
{
    public PrescriptionRepository(VitalGestDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<IEnumerable<Prescription>> GetByPatientIdAsync(
        int patientId,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Doctor)
            .Include(p => p.Items)
            .Where(p => p.PatientId == patientId && p.ClinicId == clinicId)
            .OrderByDescending(p => p.IssueDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Prescription?> GetByIdWithItemsAsync(
        int prescriptionId,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Doctor)
            .Include(p => p.Patient)
            .Include(p => p.Items.OrderBy(i => i.OrderNumber))
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == prescriptionId && p.ClinicId == clinicId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PrescriptionItem> AddItemAsync(
        PrescriptionItem item,
        CancellationToken cancellationToken = default)
    {
        await _context.PrescriptionItems.AddAsync(item, cancellationToken);
        return item;
    }

    /// <inheritdoc />
    public async Task RemoveItemAsync(int itemId, CancellationToken cancellationToken = default)
    {
        var item = await _context.PrescriptionItems.FindAsync([itemId], cancellationToken);
        if (item != null)
        {
            _context.PrescriptionItems.Remove(item);
        }
    }
}