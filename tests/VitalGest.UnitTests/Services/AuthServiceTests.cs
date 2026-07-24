using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Auth;
using VitalGest.Application.Interfaces;
using VitalGest.Application.Services;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.UnitTests.Services;

/// <summary>
/// Testes unitários para o AuthService.
/// Usa NSubstitute para mockar dependências.
/// </summary>
public class AuthServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;
    private readonly AuthService _sut; // System Under Test

    public AuthServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _tokenService = Substitute.For<ITokenService>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<AuthService>>();
        _sut = new AuthService(_uow, _tokenService, _mapper, _logger);
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnAuthResponse()
    {
        // Arrange
        var request = new RegisterRequest(
            "testuser",
            "test@email.com",
            "Senha@123",
            "Senha@123",
            "Test User"
        );

        _uow.Users.GetByEmailAsync(Arg.Any<string>()).Returns((User?)null);
        _uow.Users.GetByUsernameAsync(Arg.Any<string>()).Returns((User?)null);
        _tokenService.GenerateAccessToken(Arg.Any<User>(), Arg.Any<int?>()).Returns("fake-access-token");
        _tokenService.GenerateRefreshToken().Returns("fake-refresh-token");
        _tokenService.GetAccessTokenExpiration().Returns(DateTime.UtcNow.AddHours(2));
        _tokenService.GetRefreshTokenExpiration().Returns(DateTime.UtcNow.AddDays(7));

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be("testuser");
        result.Email.Should().Be("test@email.com");
        result.AccessToken.Should().Be("fake-access-token");
        result.RefreshToken.Should().Be("fake-refresh-token");

        await _uow.Users.Received(1).AddAsync(Arg.Any<User>());
        await _uow.Received(2).SaveChangesAsync(); // Uma para criar, outra para refresh token
    }

    [Fact]
    public async Task Register_WithExistingEmail_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var request = new RegisterRequest(
            "testuser",
            "existing@email.com",
            "Senha@123",
            "Senha@123",
            "Test User"
        );

        var existingUser = new User { Email = "existing@email.com" };
        _uow.Users.GetByEmailAsync("existing@email.com").Returns(existingUser);

        // Act
        var act = () => _sut.RegisterAsync(request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*E-mail já cadastrado*");
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnAuthResponse()
    {
        // Arrange
        var request = new LoginRequest("test@email.com", "Senha@123");

        var user = new User
        {
            Id = 1,
            Email = "test@email.com",
            Username = "testuser",
            Name = "Test User",
            Role = UserRole.User,
            IsActive = true,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Senha@123")
        };

        _uow.Users.GetByEmailAsync("test@email.com").Returns(user);
        _tokenService.GenerateAccessToken(Arg.Any<User>()).Returns("fake-access-token");
        _tokenService.GenerateRefreshToken().Returns("fake-refresh-token");
        _tokenService.GetAccessTokenExpiration().Returns(DateTime.UtcNow.AddHours(2));
        _tokenService.GetRefreshTokenExpiration().Returns(DateTime.UtcNow.AddDays(7));

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be("testuser");
        result.Role.Should().Be("User");
    }

    [Fact]
    public async Task Login_WithWrongPassword_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var request = new LoginRequest("test@email.com", "WrongPassword");

        var user = new User
        {
            Id = 1,
            Email = "test@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Senha@123"),
            IsActive = true
        };

        _uow.Users.GetByEmailAsync("test@email.com").Returns(user);

        // Act
        var act = () => _sut.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Credenciais inválidas*");
    }

    [Fact]
    public async Task Login_WithInactiveUser_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var request = new LoginRequest("inactive@email.com", "Senha@123");

        var user = new User
        {
            Email = "inactive@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Senha@123"),
            IsActive = false
        };

        _uow.Users.GetByEmailAsync("inactive@email.com").Returns(user);

        // Act
        var act = () => _sut.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Usuário desativado*");
    }

    [Fact]
    public async Task Logout_ShouldClearRefreshToken()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            RefreshToken = "old-refresh-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
        };

        _uow.Users.GetByIdAsync(1).Returns(user);

        // Act
        await _sut.LogoutAsync(1);

        // Assert
        user.RefreshToken.Should().BeNull();
        user.RefreshTokenExpiryTime.Should().BeNull();
        await _uow.Users.Received(1).UpdateAsync(user);
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task ChangePassword_WithCorrectCurrentPassword_ShouldUpdateHash()
    {
        // Arrange
        var oldHash = BCrypt.Net.BCrypt.HashPassword("OldPass@123");
        var user = new User
        {
            Id = 1,
            PasswordHash = oldHash
        };

        var request = new ChangePasswordRequest("OldPass@123", "NewPass@456", "NewPass@456");

        _uow.Users.GetByIdAsync(1).Returns(user);

        // Act
        await _sut.ChangePasswordAsync(1, request);

        // Assert
        user.PasswordHash.Should().NotBe(oldHash); // Hash deve ter mudado
        await _uow.Users.Received(1).UpdateAsync(user);
    }
}