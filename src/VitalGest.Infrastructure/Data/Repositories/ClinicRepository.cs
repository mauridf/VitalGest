using Microsoft.EntityFrameworkCore;
using VitalGest.Core.Entities;
using VitalGest.Core.Interfaces;
using VitalGest.Infrastructure.Data.Context;

namespace VitalGest.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório especializado para Clínicas.
/// </summary>
public class ClinicRepository : Repository<Clinic>, IClinicRepository
{
    public ClinicRepository(VitalGestDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<Clinic?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CNPJ == cnpj, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Clinic?> GetByIdWithDetailsAsync(int clinicId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .Include(c => c.Address)
            .Include(c => c.Departments.Where(d => d.IsActive))
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == clinicId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsActiveAsync(int clinicId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .AnyAsync(c => c.Id == clinicId && c.IsActive, cancellationToken);
    }
}