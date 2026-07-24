using System.Diagnostics;

namespace VitalGest.Api.Middlewares;

public class ResponseTimeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseTimeMiddleware> _logger;

    public ResponseTimeMiddleware(RequestDelegate next, ILogger<ResponseTimeMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        context.Response.OnStarting(() =>
        {
            stopwatch.Stop();
            context.Response.Headers["X-Response-Time-Ms"] = stopwatch.ElapsedMilliseconds.ToString();
            return Task.CompletedTask;
        });

        await _next(context);

        if (stopwatch.ElapsedMilliseconds > 3000)
        {
            _logger.LogWarning(
                "[{CorrelationId}] Requisição lenta: {Method} {Path} → {Elapsed}ms",
                context.Items["CorrelationId"],
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds);
        }
    }
}

public static class ResponseTimeMiddlewareExtensions
{
    public static IApplicationBuilder UseResponseTime(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ResponseTimeMiddleware>();
    }
}
