namespace VitalGest.Application.DTOs.Auth;

/// <summary>
/// DTO de resposta para autenticação bem-sucedida.
/// Contém tokens e dados básicos do usuário.
/// </summary>
public record AuthResponse(
    int UserId,
    string Username,
    string Email,
    string Name,
    string Role,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);