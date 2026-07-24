using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Entities;

namespace VitalGest.Application.Services;

/// <summary>
/// Serviço responsável pela geração e validação de tokens JWT.
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expireMinutes;
    private readonly int _refreshTokenExpireDays;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _secret = _configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT Secret não configurado.");
        _issuer = _configuration["Jwt:Issuer"] ?? "VitalGest";
        _audience = _configuration["Jwt:Audience"] ?? "vitalgest-api";
        _expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "120");
        _refreshTokenExpireDays = int.Parse(_configuration["Jwt:RefreshTokenExpireDays"] ?? "7");
    }

    /// <inheritdoc />
    public string GenerateAccessToken(User user, int? clinicId = null)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("name", user.Name),
            new("role", user.Role.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // Adiciona o ClinicId se fornecido (para contexto multi-tenant)
        if (clinicId.HasValue)
        {
            claims.Add(new Claim("clinic_id", clinicId.Value.ToString()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: GetAccessTokenExpiration(),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <inheritdoc />
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <inheritdoc />
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret)),
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = false, // Ignora expiração para validar token expirado
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc />
    public DateTime GetAccessTokenExpiration() => DateTime.UtcNow.AddMinutes(_expireMinutes);

    /// <inheritdoc />
    public DateTime GetRefreshTokenExpiration() => DateTime.UtcNow.AddDays(_refreshTokenExpireDays);
}