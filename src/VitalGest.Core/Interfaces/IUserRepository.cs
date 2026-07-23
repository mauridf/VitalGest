using VitalGest.Core.Entities;

namespace VitalGest.Core.Interfaces;

/// <summary>
/// Repositório especializado para Usuários.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>Busca usuário por email (inclui entidades relacionadas)</summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Busca usuário por username</summary>
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>Busca usuário por CPF</summary>
    Task<User?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);

    /// <summary>Busca usuário com vínculos de clínica</summary>
    Task<User?> GetByIdWithClinicsAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>Busca usuário por refresh token</summary>
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>Lista usuários de uma clínica específica (com paginação)</summary>
    Task<IEnumerable<User>> GetByClinicIdAsync(
        int clinicId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}