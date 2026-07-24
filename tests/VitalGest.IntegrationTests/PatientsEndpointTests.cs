using System.Text.Json;
using VitalGest.Application.DTOs.Auth;
using VitalGest.Application.DTOs.Patients;

namespace VitalGest.IntegrationTests;

/// <summary>
/// Testes de integração para os endpoints de Pacientes.
/// </summary>
public class PatientsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public PatientsEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPatients_WithoutAuth_ShouldReturn401()
    {
        var response = await _client.GetAsync("/api/Patients");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPatients_WithAuth_ShouldReturn200()
    {
        var token = await GetAuthTokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Patients");
        request.Headers.Authorization = new("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateAndGetPatient_ShouldWork()
    {
        var token = await GetAuthTokenAsync();
        var uniqueCpf = $"000.{Random.Shared.Next(100, 999)}.{Random.Shared.Next(100, 999)}-{Random.Shared.Next(10, 99)}";
        var createRequest = new { name = "Integration Test Patient", phone = "(11) 99999-9999", cpf = uniqueCpf };

        // Create
        var createMsg = new HttpRequestMessage(HttpMethod.Post, "/api/Patients")
        {
            Content = JsonContent.Create(createRequest)
        };
        createMsg.Headers.Authorization = new("Bearer", token);
        var createResponse = await _client.SendAsync(createMsg);

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK); // 200 via OkResponse

        // Get all
        var getMsg = new HttpRequestMessage(HttpMethod.Get, "/api/Patients");
        getMsg.Headers.Authorization = new("Bearer", token);
        var getResponse = await _client.SendAsync(getMsg);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var uniqueId = Guid.NewGuid().ToString()[..8];
        var registerRequest = new
        {
            username = $"inttest_{uniqueId}",
            email = $"inttest_{uniqueId}@email.com",
            password = "Test@123456",
            confirmPassword = "Test@123456",
            name = "Integration Test User"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/Auth/register", registerRequest);
        if (!registerResponse.IsSuccessStatusCode)
        {
            // Tenta login se já existir
            var loginRequest = new { email = registerRequest.email, password = registerRequest.password };
            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginContent = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
            return loginContent.GetProperty("accessToken").GetString()!;
        }

        var content = await registerResponse.Content.ReadFromJsonAsync<JsonElement>();
        return content.GetProperty("accessToken").GetString()!;
    }
}
