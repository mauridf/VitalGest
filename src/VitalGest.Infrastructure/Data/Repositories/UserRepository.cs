using Microsoft.EntityFrameworkCore;
using VitalGest.Core.Entities;
using VitalGest.Core.Interfaces;
using VitalGest.Infrastructure.Data.Context;

namespace VitalGest.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório especializado para Usuários.
/// Implementa consultas específicas além do CRUD genérico.
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(VitalGestDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower(), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return null;

        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.CPF == cpf, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> GetByIdWithClinicsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.ClinicUsers)
                .ThenInclude(cu => cu.Clinic)
            .Include(u => u.ClinicUsers)
                .ThenInclude(cu => cu.Position)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return null;

        return await _dbSet
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetByClinicIdAsync(
        int clinicId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        return await _context.ClinicUsers
            .Where(cu => cu.ClinicId == clinicId && cu.IsActive)
            .Include(cu => cu.User)
            .Include(cu => cu.Position)
            .Include(cu => cu.Department)
            .OrderBy(cu => cu.User.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(cu => cu.User)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}