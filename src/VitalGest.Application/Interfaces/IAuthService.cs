using VitalGest.Application.DTOs.Auth;

namespace VitalGest.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default);
    Task LogoutAsync(int userId, CancellationToken ct = default);
    Task<UserProfileResponse> GetProfileAsync(int userId, CancellationToken ct = default);
    Task UpdateProfileAsync(int userId, string name, string? phone, CancellationToken ct = default);
    Task ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken ct = default);
}