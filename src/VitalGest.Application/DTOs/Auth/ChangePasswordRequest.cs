namespace VitalGest.Application.DTOs.Auth;

/// <summary>
/// DTO para requisição de alteração de senha.
/// </summary>
public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
);