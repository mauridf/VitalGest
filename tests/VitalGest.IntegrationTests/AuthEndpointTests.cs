using VitalGest.Application.DTOs.Auth;

namespace VitalGest.IntegrationTests;

/// <summary>
/// Testes de integração para fluxo de autenticação.
/// </summary>
public class AuthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturn201Created()
    {
        // Arrange
        var request = new RegisterRequest(
            $"testuser_{Guid.NewGuid():N}",
            $"test_{Guid.NewGuid():N}@email.com",
            "Senha@123",
            "Senha@123",
            "Test User"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadFromJsonAsync<dynamic>();
        ((string)content!.Success).Should().Be("True");
    }

    [Fact]
    public async Task Register_WithWeakPassword_ShouldReturn400BadRequest()
    {
        // Arrange
        var request = new RegisterRequest(
            $"testuser_{Guid.NewGuid():N}",
            $"test_{Guid.NewGuid():N}@email.com",
            "123",  // Senha muito fraca
            "123",
            "Test User"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturn409Conflict()
    {
        // Arrange
        var request = new LoginRequest("nonexistent@email.com", "WrongPassword");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task AccessProtectedEndpoint_WithoutToken_ShouldReturn401Unauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/auth/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}