namespace VitalGest.Application.DTOs.Auth;

/// <summary>
/// DTO para requisição de refresh do token JWT.
/// </summary>
public record RefreshTokenRequest(
    string AccessToken,
    string RefreshToken
);