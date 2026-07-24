namespace VitalGest.Application.DTOs.Auth;

/// <summary>
/// DTO para requisição de registro de novo usuário.
/// </summary>
public record RegisterRequest(
    string Username,
    string Email,
    string Password,
    string ConfirmPassword,
    string Name,
    string? CPF = null,
    string? Phone = null
);