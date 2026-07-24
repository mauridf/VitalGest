using System.Text.Json;

namespace VitalGest.IntegrationTests;

/// <summary>
/// Testes de integração para os endpoints de Agendamentos.
/// </summary>
public class AppointmentsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AppointmentsEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAppointments_WithoutAuth_ShouldReturn401()
    {
        var response = await _client.GetAsync("/api/Appointments");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAppointments_WithAuth_ShouldReturn200()
    {
        var token = await GetAuthTokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Appointments");
        request.Headers.Authorization = new("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSchedule_WithoutAuth_ShouldReturn401()
    {
        var response = await _client.GetAsync("/api/Schedule/slots?doctorId=1&date=2026-07-23");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetClinicStats_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/Clinics/1/stats");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var uniqueId = Guid.NewGuid().ToString()[..8];
        var registerRequest = new
        {
            username = $"intapp_{uniqueId}",
            email = $"intapp_{uniqueId}@email.com",
            password = "Test@123456",
            confirmPassword = "Test@123456",
            name = "Integration Test User"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/Auth/register", registerRequest);
        if (!registerResponse.IsSuccessStatusCode)
        {
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
