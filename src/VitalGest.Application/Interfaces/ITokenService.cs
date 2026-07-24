using System.Security.Claims;
using VitalGest.Core.Entities;

namespace VitalGest.Application.Interfaces;

/// <summary>
/// Serviço responsável pela geração e validação de tokens JWT.
/// </summary>
public interface ITokenService
{
    /// <summary>Gera access token JWT para o usuário</summary>
    string GenerateAccessToken(User user, int? clinicId = null);

    /// <summary>Gera refresh token (string aleatória)</summary>
    string GenerateRefreshToken();

    /// <summary>Obtém claims principais do token</summary>
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    /// <summary>Calcula data de expiração do access token</summary>
    DateTime GetAccessTokenExpiration();

    /// <summary>Calcula data de expiração do refresh token</summary>
    DateTime GetRefreshTokenExpiration();
}