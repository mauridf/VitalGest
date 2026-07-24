namespace VitalGest.Application.DTOs.Auth;

/// <summary>
/// DTO para requisição de login.
/// Aceita email ou username como identificador.
/// </summary>
public record LoginRequest(
    string Login,    // Pode ser email ou username
    string Password
);