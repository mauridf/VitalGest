using AutoMapper;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Auth;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;
using VitalGest.Core.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

/// <summary>
/// Serviço de autenticação: registro, login, refresh token, logout, perfil.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork uow,
        ITokenService tokenService,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _uow = uow;
        _tokenService = tokenService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Registrando novo usuário: {Username}", request.Username);

        // Verifica se email já existe
        if (await _uow.Users.GetByEmailAsync(request.Email, ct) != null)
            throw new BusinessRuleException("E-mail já cadastrado.", "EMAIL_ALREADY_EXISTS");

        // Verifica se username já existe
        if (await _uow.Users.GetByUsernameAsync(request.Username, ct) != null)
            throw new BusinessRuleException("Username já está em uso.", "USERNAME_ALREADY_EXISTS");

        // Verifica CPF se informado
        if (!string.IsNullOrEmpty(request.CPF))
        {
            if (await _uow.Users.GetByCpfAsync(request.CPF, ct) != null)
                throw new BusinessRuleException("CPF já cadastrado.", "CPF_ALREADY_EXISTS");
        }

        // Cria usuário
        var user = new User
        {
            Username = request.Username,
            Email = request.Email.ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Name = request.Name,
            CPF = request.CPF,
            Phone = request.Phone,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        // Gera tokens
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Salva refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = _tokenService.GetRefreshTokenExpiration();
        await _uow.Users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Usuário registrado com sucesso: {UserId}", user.Id);

        return new AuthResponse(
            user.Id,
            user.Username,
            user.Email,
            user.Name,
            user.Role.ToString(),
            accessToken,
            refreshToken,
            _tokenService.GetAccessTokenExpiration()
        );
    }

    /// <inheritdoc />
    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Tentativa de login: {Login}", request.Login);

        // Busca por email ou username
        var user = await _uow.Users.GetByEmailAsync(request.Login, ct)
                   ?? await _uow.Users.GetByUsernameAsync(request.Login, ct);

        if (user == null)
            throw new BusinessRuleException("Credenciais inválidas.", "INVALID_CREDENTIALS");

        // Verifica se usuário está ativo
        if (!user.IsActive)
            throw new BusinessRuleException("Usuário desativado. Entre em contato com o administrador.", "USER_INACTIVE");

        // Verifica senha
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new BusinessRuleException("Credenciais inválidas.", "INVALID_CREDENTIALS");

        // Gera tokens
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Atualiza refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = _tokenService.GetRefreshTokenExpiration();
        await _uow.Users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Login bem-sucedido: {UserId}", user.Id);

        return new AuthResponse(
            user.Id,
            user.Username,
            user.Email,
            user.Name,
            user.Role.ToString(),
            accessToken,
            refreshToken,
            _tokenService.GetAccessTokenExpiration()
        );
    }

    /// <inheritdoc />
    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default)
    {
        // Valida o access token expirado
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            throw new BusinessRuleException("Token de acesso inválido.", "INVALID_ACCESS_TOKEN");

        var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)
            ?? principal.FindFirst("sub");

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            throw new BusinessRuleException("Token de acesso inválido.", "INVALID_ACCESS_TOKEN");

        // Busca usuário
        var user = await _uow.Users.GetByIdAsync(userId, ct);
        if (user == null)
            throw new NotFoundException("Usuário", userId);

        // Verifica refresh token
        if (user.RefreshToken != request.RefreshToken)
            throw new BusinessRuleException("Refresh token inválido.", "INVALID_REFRESH_TOKEN");

        if (user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            throw new BusinessRuleException("Refresh token expirado. Faça login novamente.", "REFRESH_TOKEN_EXPIRED");

        // Gera novos tokens
        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = _tokenService.GetRefreshTokenExpiration();
        await _uow.Users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        return new AuthResponse(
            user.Id,
            user.Username,
            user.Email,
            user.Name,
            user.Role.ToString(),
            newAccessToken,
            newRefreshToken,
            _tokenService.GetAccessTokenExpiration()
        );
    }

    /// <inheritdoc />
    public async Task LogoutAsync(int userId, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdAsync(userId, ct);
        if (user == null)
            throw new NotFoundException("Usuário", userId);

        // Invalida refresh token
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _uow.Users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Logout realizado: {UserId}", userId);
    }

    /// <inheritdoc />
    public async Task<UserProfileResponse> GetProfileAsync(int userId, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdWithClinicsAsync(userId, ct);
        if (user == null)
            throw new NotFoundException("Usuário", userId);

        var profile = new UserProfileResponse(
            user.Id,
            user.Username,
            user.Email,
            user.Name,
            user.CPF,
            user.Phone,
            user.AvatarUrl,
            user.Role.ToString(),
            user.CreatedAt,
            user.ClinicUsers.Select(cu => new ClinicInfoResponse(
                cu.ClinicId,
                cu.Clinic.Name,
                cu.Position.Name,
                cu.Department?.Name
            ))
        );

        return profile;
    }

    /// <inheritdoc />
    public async Task UpdateProfileAsync(int userId, string name, string? phone, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdAsync(userId, ct);
        if (user == null)
            throw new NotFoundException("Usuário", userId);

        user.Name = name;
        user.Phone = phone;
        user.UpdatedAt = DateTime.UtcNow;

        await _uow.Users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdAsync(userId, ct);
        if (user == null)
            throw new NotFoundException("Usuário", userId);

        // Verifica senha atual
        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw new BusinessRuleException("Senha atual incorreta.", "INVALID_CURRENT_PASSWORD");

        // Atualiza senha
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _uow.Users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Senha alterada: {UserId}", userId);
    }
}