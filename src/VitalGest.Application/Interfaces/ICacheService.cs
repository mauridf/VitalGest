namespace VitalGest.Application.Interfaces;

/// <summary>
/// Contrato para serviço de cache distribuído.
/// Abstrai operações de cache (Redis em produção, In-Memory em desenvolvimento).
/// </summary>
public interface ICacheService
{
    /// <summary>Obtém valor do cache</summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>Armazena valor no cache com TTL opcional</summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>Remove uma chave do cache</summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>Verifica se uma chave existe no cache</summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>Remove todas as chaves com determinado prefixo</summary>
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
}