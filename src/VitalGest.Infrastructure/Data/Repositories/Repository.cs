using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VitalGest.Core.Interfaces;
using VitalGest.Infrastructure.Data.Context;

namespace VitalGest.Infrastructure.Data.Repositories;

/// <summary>
/// Implementação genérica do padrão Repository.
/// Fornece operações CRUD básicas e consultas comuns para qualquer entidade.
/// </summary>
/// <typeparam name="T">Tipo da entidade (deve ser uma classe)</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly VitalGestDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(VitalGestDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        // Verifica se a entidade já está sendo rastreada
        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
            entry.State = EntityState.Modified;
        }
        else
        {
            entry.State = EntityState.Modified;
        }

        // Atualiza o campo UpdatedAt se a entidade tiver essa propriedade
        var updatedAtProperty = typeof(T).GetProperty("UpdatedAt");
        if (updatedAtProperty != null && updatedAtProperty.CanWrite)
        {
            updatedAtProperty.SetValue(entity, DateTime.UtcNow);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        // Soft delete: se a entidade tiver IsActive, apenas desativa
        var isActiveProperty = typeof(T).GetProperty("IsActive");
        if (isActiveProperty != null && isActiveProperty.PropertyType == typeof(bool))
        {
            isActiveProperty.SetValue(entity, false);
            _dbSet.Update(entity);
        }
        else
        {
            // Hard delete para entidades sem soft delete (ex: AuditLog, Notification)
            _dbSet.Remove(entity);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            return await _dbSet.CountAsync(cancellationToken);

        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        // Garante que page e pageSize são válidos
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        IQueryable<T> query = _dbSet.AsNoTracking();

        if (predicate != null)
            query = query.Where(predicate);

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}