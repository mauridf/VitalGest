using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VitalGest.Application.DTOs.Auth;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de autenticação: registro, login, refresh token, logout e perfil.
/// </summary>
[EnableRateLimiting("Auth")] // Rate limiting mais restritivo para auth
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registra um novo usuário no sistema.
    /// </summary>
    /// <param name="request">Dados do novo usuário</param>
    /// <returns>Tokens de acesso e dados do usuário</returns>
    /// <response code="201">Usuário registrado com sucesso</response>
    /// <response code="409">Email, username ou CPF já cadastrado</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return CreatedAtAction(nameof(GetProfile), null, new
        {
            Success = true,
            Message = "Usuário registrado com sucesso.",
            Data = result
        });
    }

    /// <summary>
    /// Realiza login do usuário.
    /// Aceita email ou username como identificador.
    /// </summary>
    /// <param name="request">Credenciais de login</param>
    /// <returns>Tokens de acesso e dados do usuário</returns>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="409">Credenciais inválidas</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return OkResponse(result, "Login realizado com sucesso.");
    }

    /// <summary>
    /// Renova o access token usando o refresh token.
    /// </summary>
    /// <param name="request">Access token expirado e refresh token</param>
    /// <returns>Novos tokens de acesso</returns>
    /// <response code="200">Tokens renovados com sucesso</response>
    /// <response code="409">Refresh token inválido ou expirado</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        return OkResponse(result, "Token renovado com sucesso.");
    }

    /// <summary>
    /// Realiza logout do usuário, invalidando o refresh token.
    /// </summary>
    /// <returns>Confirmação de logout</returns>
    /// <response code="200">Logout realizado com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Logout()
    {
        var userId = GetUserId();
        await _authService.LogoutAsync(userId);
        return OkResponse(new { }, "Logout realizado com sucesso.");
    }

    /// <summary>
    /// Obtém o perfil do usuário logado.
    /// Inclui dados pessoais e vínculos com clínicas.
    /// </summary>
    /// <returns>Dados completos do perfil</returns>
    /// <response code="200">Perfil obtido com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetUserId();
        var result = await _authService.GetProfileAsync(userId);
        return OkResponse(result);
    }

    /// <summary>
    /// Atualiza dados do perfil do usuário logado.
    /// </summary>
    /// <param name="request">Novos dados do perfil</param>
    /// <returns>Confirmação de atualização</returns>
    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(200)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = GetUserId();
        await _authService.UpdateProfileAsync(userId, request.Name, request.Phone);
        return OkResponse(new { }, "Perfil atualizado com sucesso.");
    }

    /// <summary>
    /// Altera a senha do usuário logado.
    /// </summary>
    /// <param name="request">Senha atual e nova senha</param>
    /// <returns>Confirmação de alteração</returns>
    /// <response code="200">Senha alterada com sucesso</response>
    /// <response code="409">Senha atual incorreta</response>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = GetUserId();
        await _authService.ChangePasswordAsync(userId, request);
        return OkResponse(new { }, "Senha alterada com sucesso.");
    }
}

/// <summary>
/// DTO para atualização de perfil.
/// </summary>
public record UpdateProfileRequest(string Name, string? Phone);