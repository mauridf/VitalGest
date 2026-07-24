namespace VitalGest.Application.DTOs.Auth;

/// <summary>
/// DTO com dados do perfil do usuário logado.
/// </summary>
public record UserProfileResponse(
    int Id,
    string Username,
    string Email,
    string Name,
    string? CPF,
    string? Phone,
    string? AvatarUrl,
    string Role,
    DateTime CreatedAt,
    IEnumerable<ClinicInfoResponse> Clinics
);

/// <summary>
/// Informações resumidas da clínica para o perfil do usuário.
/// </summary>
public record ClinicInfoResponse(
    int ClinicId,
    string ClinicName,
    string Position,
    string? Department
);