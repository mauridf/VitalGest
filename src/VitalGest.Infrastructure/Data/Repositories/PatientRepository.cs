using Microsoft.EntityFrameworkCore;
using VitalGest.Core.Entities;
using VitalGest.Core.Interfaces;
using VitalGest.Infrastructure.Data.Context;

namespace VitalGest.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório especializado para Pacientes.
/// Implementa buscas textuais e consultas com relacionamentos.
/// </summary>
public class PatientRepository : Repository<Patient>, IPatientRepository
{
    public PatientRepository(VitalGestDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<Patient?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return null;

        // Ignora o filtro de tenant para buscar CPF globalmente
        return await _dbSet
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.CPF == cpf, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Patient>> SearchByNameAsync(
        string query,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Enumerable.Empty<Patient>();

        // Usa EF.Functions.ILike para busca case-insensitive com suporte a pg_trgm
        return await _dbSet
            .Where(p => p.ClinicId == clinicId && p.IsActive)
            .Where(p => EF.Functions.ILike(p.Name, $"%{query}%"))
            .OrderBy(p => p.Name)
            .Take(20)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Patient>> SearchAsync(
        string query,
        int clinicId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Enumerable.Empty<Patient>();

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        // Busca por nome OU CPF OU telefone
        return await _dbSet
            .Where(p => p.ClinicId == clinicId && p.IsActive)
            .Where(p =>
                EF.Functions.ILike(p.Name, $"%{query}%") ||
                (p.CPF != null && p.CPF.Contains(query)) ||
                p.Phone.Contains(query))
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Patient>> GetByInsurancePlanIdAsync(
        int insurancePlanId,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.ClinicId == clinicId && p.InsurancePlanId == insurancePlanId && p.IsActive)
            .OrderBy(p => p.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Patient?> GetByIdWithDetailsAsync(
        int patientId,
        int clinicId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Address)
            .Include(p => p.InsurancePlan)
            .Include(p => p.MedicalRecord)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == patientId && p.ClinicId == clinicId, cancellationToken);
    }
}