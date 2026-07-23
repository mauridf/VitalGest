using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using VitalGest.Application.Interfaces;

namespace VitalGest.Infrastructure.Services;

/// <summary>
/// Serviço de cache distribuído usando Redis.
/// Implementa operações básicas de cache com serialização JSON.
/// </summary>
public class CacheService : ICacheService
{
    private readonly IDatabase _redisDb;
    private readonly ILogger<CacheService> _logger;

    // Prefixo para evitar colisão de chaves
    private const string KeyPrefix = "vitalgest:";

    public CacheService(IConnectionMultiplexer redis, ILogger<CacheService> logger)
    {
        _redisDb = redis.GetDatabase();
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var value = await _redisDb.StringGetAsync(GetFullKey(key));
            if (value.IsNull)
                return null;

            return JsonSerializer.Deserialize<T>((string)value!);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao obter cache para a chave: {Key}", key);
            return null; // Falha de cache não deve quebrar a aplicação
        }
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var serialized = JsonSerializer.Serialize(value);
            await _redisDb.StringSetAsync(
                GetFullKey(key),
                serialized,
                expiration ?? TimeSpan.FromMinutes(5) // TTL padrão: 5 minutos
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao definir cache para a chave: {Key}", key);
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _redisDb.KeyDeleteAsync(GetFullKey(key));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao remover cache para a chave: {Key}", key);
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _redisDb.KeyExistsAsync(GetFullKey(key));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao verificar existência de cache para a chave: {Key}", key);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        try
        {
            var server = _redisDb.Multiplexer.GetServer(
                _redisDb.Multiplexer.GetEndPoints().First());

            var keys = server.Keys(pattern: $"{KeyPrefix}{prefix}*");

            foreach (var key in keys)
            {
                await _redisDb.KeyDeleteAsync(key);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao remover cache por prefixo: {Prefix}", prefix);
        }
    }

    /// <summary>
    /// Obtém a chave completa com prefixo do sistema.
    /// </summary>
    private static string GetFullKey(string key) => $"{KeyPrefix}{key}";
}