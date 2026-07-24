using Microsoft.Extensions.Configuration;
using VitalGest.Application.Services;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;

namespace VitalGest.UnitTests.Services;

/// <summary>
/// Testes unitários para o TokenService.
/// </summary>
public class TokenServiceTests
{
    private readonly TokenService _sut;
    private readonly IConfiguration _configuration;

    public TokenServiceTests()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Secret", "ThisIsASuperSecretKeyForTestingWith32Characters!" },
            { "Jwt:Issuer", "VitalGest" },
            { "Jwt:Audience", "vitalgest-api" },
            { "Jwt:ExpireMinutes", "120" },
            { "Jwt:RefreshTokenExpireDays", "7" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _sut = new TokenService(_configuration);
    }

    [Fact]
    public void GenerateAccessToken_ShouldReturnValidToken()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@email.com",
            Name = "Test User",
            Role = UserRole.Admin
        };

        // Act
        var token = _sut.GenerateAccessToken(user, 1);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Length.Should().Be(3); // JWT tem 3 partes (header.payload.signature)
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnUniqueToken()
    {
        // Act
        var token1 = _sut.GenerateRefreshToken();
        var token2 = _sut.GenerateRefreshToken();

        // Assert
        token1.Should().NotBeNullOrEmpty();
        token2.Should().NotBeNullOrEmpty();
        token1.Should().NotBe(token2); // Devem ser diferentes
        token1.Length.Should().BeGreaterThan(32); // Base64 de 64 bytes
    }

    [Fact]
    public void GetAccessTokenExpiration_ShouldReturnFutureDate()
    {
        // Act
        var expiration = _sut.GetAccessTokenExpiration();

        // Assert
        expiration.Should().BeAfter(DateTime.UtcNow);
        expiration.Should().BeCloseTo(DateTime.UtcNow.AddHours(2), TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ShouldReturnPrincipal()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@email.com", Name = "Test", Role = UserRole.User };
        var token = _sut.GenerateAccessToken(user);

        // Act
        var principal = _sut.GetPrincipalFromExpiredToken(token);

        // Assert
        principal.Should().NotBeNull();
        var subClaim = principal!.FindFirst("sub");
        subClaim.Should().NotBeNull();
        subClaim!.Value.Should().Be("1");
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_WithInvalidToken_ShouldReturnNull()
    {
        // Act
        var principal = _sut.GetPrincipalFromExpiredToken("invalid-token");

        // Assert
        principal.Should().BeNull();
    }
}