using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Endpoint público para health check e informações da API.
/// </summary>
public class HealthController : BaseApiController
{
    private readonly IConfiguration _configuration;

    public HealthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Health check da API.
    /// Retorna status, versão e ambiente.
    /// </summary>
    [HttpGet("/api/health")]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "Healthy",
            Name = "VitalGest API",
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            Timestamp = DateTime.UtcNow,
            Database = "PostgreSQL 16",
            Cache = "Redis"
        });
    }
}