using System.Diagnostics;

namespace VitalGest.Api.Middlewares;

/// <summary>
/// Middleware que registra informações de cada requisição:
/// método HTTP, path, status code e tempo de resposta.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? "N/A";

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var statusCode = context.Response.StatusCode;
            var method = context.Request.Method;
            var path = context.Request.Path;
            var elapsed = stopwatch.ElapsedMilliseconds;

            // Nível de log baseado no status code
            if (statusCode >= 500)
            {
                _logger.LogError(
                    "[{CorrelationId}] {Method} {Path} → {StatusCode} ({Elapsed}ms)",
                    correlationId, method, path, statusCode, elapsed);
            }
            else if (statusCode >= 400)
            {
                _logger.LogWarning(
                    "[{CorrelationId}] {Method} {Path} → {StatusCode} ({Elapsed}ms)",
                    correlationId, method, path, statusCode, elapsed);
            }
            else
            {
                _logger.LogInformation(
                    "[{CorrelationId}] {Method} {Path} → {StatusCode} ({Elapsed}ms)",
                    correlationId, method, path, statusCode, elapsed);
            }
        }
    }
}

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}