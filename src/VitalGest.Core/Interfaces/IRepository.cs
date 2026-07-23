using System.Linq.Expressions;

namespace VitalGest.Core.Interfaces;

/// <summary>
/// Interface genérica para repositórios.
/// Fornece operações CRUD básicas e consultas comuns.
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>Busca entidade por ID</summary>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Lista todas as entidades</summary>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Busca entidades com filtro</summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Adiciona nova entidade</summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Atualiza entidade existente</summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Remove entidade (soft delete quando aplicável)</summary>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Verifica se existe entidade com o filtro</summary>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Conta entidades com filtro</summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>Lista paginada com filtro opcional</summary>
    Task<IEnumerable<T>> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
}