namespace VitalGest.Api.Middlewares;

/// <summary>
/// Middleware que adiciona headers de segurança em todas as respostas.
/// Segue recomendações OWASP para hardening de API.
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Adiciona headers de segurança
        var headers = context.Response.Headers;

        // Previne MIME type sniffing
        headers["X-Content-Type-Options"] = "nosniff";

        // Previne clickjacking
        headers["X-Frame-Options"] = "DENY";

        // Habilita filtro XSS do navegador
        headers["X-XSS-Protection"] = "1; mode=block";

        // Política de referrer
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // Permissions Policy (desabilita recursos não utilizados)
        headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";

        // Remove header que expõe tecnologia
        headers.Remove("X-Powered-By");

        await _next(context);
    }
}

public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeadersMiddleware>();
    }
}